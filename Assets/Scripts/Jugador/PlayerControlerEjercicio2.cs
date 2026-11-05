using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControllerEjercicio2 : MonoBehaviour
{
    private int vidas = 3;
    private int puntuacion = 0;
    private float tiempoTotal = 0;
    private int contadorSaltos = 2;
    private float ultimoSalto = 0;
    private float movimientoX;
    private bool tocandoSuelo;
    private bool tieneLlave;
    private Vector2 ultimoCheckpoint = new Vector2(-8, -4);
    private Rigidbody2D rb;

    [SerializeField] float velocidadX = 5f;
    [SerializeField] float velocidadY = 5f;
    [SerializeField] TextMeshProUGUI panelPuntuacion;
    [SerializeField] TextMeshProUGUI tiempotranscurrido;
    [SerializeField] TextMeshProUGUI advertenciaTxt;
    [SerializeField] TextMeshProUGUI victoriaTxt;
    [SerializeField] Slider vidasSlider;
    [SerializeField] TextMeshProUGUI saltosTxt;
    [SerializeField] GameObject gameOver;
    [SerializeField] GameObject gameUI;
    [SerializeField] GameObject menuPausa;
    [SerializeField] GameObject mensajeCuidado;

    void Start()
    {
        vidas = 3;
        Time.timeScale=1f;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //En cada frame mira si hay que mover al jugador
        movimientoX = Input.GetAxis("Horizontal");
        
        if(Input.GetKeyDown(KeyCode.Space) && tocandoSuelo)
        {
            rb.AddForce(new Vector2(0, velocidadY), ForceMode2D.Impulse);
            ultimoSalto = tiempoTotal;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && !tocandoSuelo && contadorSaltos!= 0 && ultimoSalto < tiempoTotal - 0.5)
        {
            rb.AddForce(new Vector2(0, velocidadY), ForceMode2D.Impulse);
            contadorSaltos--;
            saltosTxt.text = "Saltos: " + contadorSaltos;
            ultimoSalto = tiempoTotal;
        }
        else
        {
            rb.AddForce(new Vector2(movimientoX * velocidadX, 0));
        }

        //controlar el tiempo pasado
        tiempoTotal = tiempoTotal + Time.deltaTime;
        tiempotranscurrido.text = "Tiempo: " + tiempoTotal.ToString("F2");
        
        if (vidas == 0)
        {   
            gameOver.SetActive(true);
            gameUI.SetActive(false);
            Time.timeScale=0f;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0f;
            menuPausa.SetActive(true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bola"))
        {
            collision.gameObject.SetActive(false);
            puntuacion++;
            panelPuntuacion.text = "Puntuacion: " + puntuacion.ToString();
        }
        
        if (collision.gameObject.CompareTag("Suelo"))
        {
            tocandoSuelo = true;
            contadorSaltos = 2;
            saltosTxt.text = "Saltos: " + contadorSaltos;
        }

        if (collision.gameObject.CompareTag("NoSaltar"))
        {
            tocandoSuelo = false;
        }
        
        if (collision.gameObject.CompareTag("Pincho") || collision.gameObject.CompareTag("ZonaMuerte"))
        {
            vidas = 0;
            vidasSlider.value = vidas;
            MoverUltimoCheckpoint(ultimoCheckpoint);
        }

        if (collision.gameObject.CompareTag("Enemigo"))
        {
            vidas--;
            vidasSlider.value = vidas;
            MoverUltimoCheckpoint(ultimoCheckpoint);
        }

        if (collision.gameObject.CompareTag("Puerta"))
        {
            if (tieneLlave== true)
            {
                collision.gameObject.SetActive(false);
            }
        }

        if (collision.gameObject.CompareTag("CamaElastica") && rb.linearVelocity.y <= 0f)
        {
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10f);
            }
        }

        if (collision.gameObject.CompareTag("Meta") && puntuacion == 10)
        {
            victoriaTxt.text = "Has ganado";
            Time.timeScale=0f;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            tocandoSuelo = false;
            contadorSaltos--;
            saltosTxt.text = "Saltos: " + contadorSaltos;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Moneda"))
        {
            collision.gameObject.SetActive(false);
            puntuacion++;
            panelPuntuacion.text = "Puntuacion: " + puntuacion.ToString();
        }
        
        if (collision.gameObject.CompareTag("ParedInvisible"))
        {
            Debug.Log("Detecto que toca la pared invisible");
        }

        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            ultimoCheckpoint = new Vector2(collision.transform.position.x, collision.transform.position.y);
        }

        if (collision.gameObject.CompareTag("Advertencia"))
        {
            advertenciaTxt.text = "Cuidado con los pinchos";
        }

        if (collision.gameObject.CompareTag("Llave"))
        {
            collision.gameObject.SetActive(false);
            tieneLlave = true;
        }

        if (collision.gameObject.CompareTag("CabezaEnemigo"))
        {
            collision.gameObject.transform.parent.gameObject.SetActive(false);
        }

        if (collision.gameObject.CompareTag("MensajeCuidado"))
        {
            mensajeCuidado.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Advertencia"))
        {
            advertenciaTxt.text = "";
        }

        if (collision.gameObject.CompareTag("MensajeCuidado"))
        {
            mensajeCuidado.SetActive(false);
        }
    }

    public void ReiniciarNivel()
    {
        SceneManager.LoadScene("Ejercicio1");
        Time.timeScale=1f;
    }

    public void MoverUltimoCheckpoint(Vector2 vector2)
    {
        transform.position = vector2;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public void IFrames()
    {
        
    }

    public void ResumirJuego()
    {
        Time.timeScale = 1f;
    }

    public void Salir()
    {
        SceneManager.LoadScene("PantallaInicio");
    }
}
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControllerEjercicio1 : MonoBehaviour
{
    private int vidas = 3;
    private int puntuacion = 0;
    private float tiempoTotal = 0;
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
    [SerializeField] TextMeshProUGUI vidasTxt;
    [SerializeField] GameObject gameOver;
    [SerializeField] GameObject gameUI;

    void Start()
    {
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
        }

        if (collision.gameObject.CompareTag("NoSaltar"))
        {
            tocandoSuelo = false;
        }
        
        if (collision.gameObject.CompareTag("Pincho") || collision.gameObject.CompareTag("ZonaMuerte"))
        {
            vidas--;
            vidasTxt.text = "Vidas: " + vidas.ToString();
            MoverUltimoCheckpoint(ultimoCheckpoint);
        }

        if (collision.gameObject.CompareTag("Puerta"))
        {
            if (tieneLlave== true)
            {
                collision.gameObject.SetActive(false);
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
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Advertencia"))
        {
            advertenciaTxt.text = "";
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

    public void Salir()
    {
        SceneManager.LoadScene("PantallaInicio");
    }
}
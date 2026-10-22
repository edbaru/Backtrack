using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float velocidadX = 0.1f;
    [SerializeField] float velocidadY = 0.5f;
    private bool vivo = true;
    [SerializeField] TextMeshProUGUI panelPuntuacion;
    [SerializeField] ParticleSystem sistemaParticulas;
    [SerializeField] TextMeshProUGUI tiempotranscurrido;
    [SerializeField] GameObject pantallaMuerte;
    public float movimientoX = 0f;
    public float movimientoY = 0f;
    private Rigidbody2D rb;

    private int vidas = 3;
    private bool tocandoSuelo;

    private int puntuacion = 0;

    private float tiempoTotal = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
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
            Debug.Log("Detecto que toca el trigger");
        } else if (collision.gameObject.CompareTag("ParedInvisible"))
        {
            Debug.Log("Detecto que toca la pared invisible");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Detecta salgo del trigger");
    }

    public void ReiniciarNivel()
    {
        SceneManager.LoadScene(1);
        Time.timeScale=1f;
    }

    public void Salir()
    {
        SceneManager.LoadScene(0);
    }

    public void MatarPJ()
    {
        pantallaMuerte.SetActive(true);
        Time.timeScale=0f;
    }
}
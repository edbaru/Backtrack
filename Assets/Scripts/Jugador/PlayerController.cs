using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private int vidas = 3;
    private static int monedas;
    private int llaves;
    private readonly List<string> habilidadesRecogidas = new List<string>();
    private float tiempoTotal = 0;
    private float movimientoX;
    private float movimientoY;
    private float fuerzaEscalar = 0f;
    private bool tocandoSuelo;
    private bool saltoDobleDesbloqueado;
    private bool saltoDobleActivo;
    private Vector2 ultimoCheckpoint = new Vector2(0, 0);
    private Rigidbody2D rb;

    [SerializeField] float velocidadX = 5f;
    [SerializeField] float velocidadY = 5f;
    [SerializeField] ParticleSystem efectoRecogerHabilidad;
    [SerializeField] TextMeshProUGUI vidasTxt;
    [SerializeField] TextMeshProUGUI tiempotranscurrido;
    [SerializeField] TextMeshProUGUI monedasTxt;
    [SerializeField] TextMeshProUGUI llavesTxt;
    [SerializeField] TextMeshProUGUI panelHabilidades;
    [SerializeField] GameObject gameUI;
    [SerializeField] GameObject gameWin;
    [SerializeField] GameObject gameOver;
    [SerializeField] AudioSource efectoSonidoSalto;
    [SerializeField] AudioSource efectoSonidoPowerUp;
    [SerializeField] AudioSource efectoSonidoCogerLlave;
    [SerializeField] AudioSource efectoSonidoGameWin;
    [SerializeField] AudioSource efectoSonidoGameOver;    
    [SerializeField] GameObject mensajeSalto;
    [SerializeField] GameObject mensajeTrepar;
    [SerializeField] GameObject mensajeSaltoDoble;
    [SerializeField] GameObject mensajeLlave;


    void Start()
    {
        Time.timeScale = 1f;
        rb = GetComponent<Rigidbody2D>();
        ActualizarPanelHabilidades();
    }

    void Update()
    {
        //En cada frame mira si hay que mover al jugador
        movimientoX = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && tocandoSuelo && movimientoY != 0)
        {
            rb.AddForce(new Vector2(0, movimientoY), ForceMode2D.Impulse);
            efectoSonidoSalto.Play();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && !tocandoSuelo && saltoDobleDesbloqueado && saltoDobleActivo)
        {
            rb.AddForce(new Vector2(0, movimientoY), ForceMode2D.Impulse);
            saltoDobleActivo = false;
            efectoSonidoSalto.Play();
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
            efectoSonidoGameOver.Play();
            Time.timeScale = 0f;
        }

        //Añadir menu de pausa
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            tocandoSuelo = true;
            saltoDobleActivo = false;
        }

        if (collision.gameObject.CompareTag("Pincho") || collision.gameObject.CompareTag("ZonaMuerte"))
        {
            vidas--;
            vidasTxt.text = "Vidas: " + vidas.ToString();
            MoverUltimoCheckpoint(ultimoCheckpoint);
        }

        if (collision.gameObject.CompareTag("Puerta"))
        {
            if (llaves >= 1)
            {
                collision.gameObject.SetActive(false);
                llaves--;
                if (llaves == 0)
                {
                    llavesTxt.text = "";
                }
                else
                {
                    llavesTxt.text = "Llaves: " + llaves;
                }
            }
        }

        if (collision.gameObject.CompareTag("Meta"))
        {
            gameWin.SetActive(true);
            gameUI.SetActive(false);
            efectoSonidoGameWin.Play();
        }

        if (collision.gameObject.CompareTag("ParedTrepar") && tocandoSuelo == true)
        {
            rb.AddForce(new Vector2(0, collision.transform.localScale.x * 2 * fuerzaEscalar), ForceMode2D.Impulse);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            tocandoSuelo = false;
            saltoDobleActivo = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Moneda"))
        {
            monedas++;
            monedasTxt.text = "Monedas: " + monedas;
            collision.gameObject.SetActive(false);
        }
        if (collision.gameObject.CompareTag("Llave"))
        {
            llaves++;
            llavesTxt.text = "Llaves: " + llaves;
            mensajeLlave.SetActive(true);
            collision.GetComponent<SpriteRenderer>().enabled = false;
            efectoSonidoCogerLlave.Play();
            collision.enabled = false;
        }

        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            ultimoCheckpoint = new Vector2(collision.gameObject.GetComponentInChildren<Transform>().position.x, collision.gameObject.GetComponentInChildren<Transform>().position.y);
        }

        if (collision.gameObject.CompareTag("Salto"))
        {
            movimientoY = velocidadY;
            mensajeSalto.SetActive(true);
            collision.GetComponent<SpriteRenderer>().enabled = false;
            efectoRecogerHabilidad.gameObject.SetActive(true);
            efectoRecogerHabilidad.transform.position = collision.transform.position;
            efectoRecogerHabilidad.Play();
            efectoSonidoPowerUp.Play();
            collision.enabled = false;
            RegistrarHabilidad("Salto");
        }
        if (collision.gameObject.CompareTag("Trepar"))
        {
            fuerzaEscalar = 1f;
            mensajeTrepar.SetActive(true);
            collision.GetComponent<SpriteRenderer>().enabled = false;
            efectoRecogerHabilidad.gameObject.SetActive(true);
            efectoRecogerHabilidad.transform.position = collision.transform.position;
            efectoRecogerHabilidad.Play();
            efectoSonidoPowerUp.Play();
            collision.enabled = false;
            RegistrarHabilidad("Trepar");
        }
        if (collision.gameObject.CompareTag("SaltoDoble"))
        {
            saltoDobleDesbloqueado = true;
            mensajeSaltoDoble.SetActive(true);
            collision.GetComponent<SpriteRenderer>().enabled = false;
            efectoRecogerHabilidad.gameObject.SetActive(true);
            efectoRecogerHabilidad.transform.position = collision.transform.position;
            efectoRecogerHabilidad.Play();
            efectoSonidoPowerUp.Play();
            collision.enabled = false;
            RegistrarHabilidad("Salto doble");
        }
    }

    private void RegistrarHabilidad(string habilidad)
    {
        if (!habilidadesRecogidas.Contains(habilidad))
        {
            habilidadesRecogidas.Add(habilidad);
            ActualizarPanelHabilidades();
        }
    }

    private void ActualizarPanelHabilidades()
    {
        panelHabilidades.text = "Habilidades: " +
            string.Join(", ", habilidadesRecogidas) +
            " (" + habilidadesRecogidas.Count + ")";
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Detecta salgo del trigger");
    }

    public void ReiniciarTutorial()
    {
        SceneManager.LoadScene("primeraEscena2D");
        Time.timeScale = 1f;
    }

    public void ReiniciarNivel1()
    {
        SceneManager.LoadScene("segundaEscena2D");
        Time.timeScale = 1f;
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
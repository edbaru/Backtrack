using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControllerEjercicio3 : MonoBehaviour
{
    private int vidas = 3;
    private int puntuacion = 0;
    private float tiempoTotal = 0;
    private int contadorSaltos = 2;
    private float ultimoSalto = 0;
    private float movimientoX;
    private float valorAceleracion;
    private string nombreControlActivo = "NINGUNO";
    private string textoControlActivoMostrado;
    private bool saltoSolicitado;
    private bool tocandoSuelo;
    private bool tieneLlave;
    private Vector2 ultimoCheckpoint = new Vector2(-8, -4);
    private Rigidbody2D rb;
    private InputActionMap accionesJugador;
    private InputAction accionPausa;
    private InputAction accionSuperAtaque;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [SerializeField] float velocidadX = 5f;
    [SerializeField] float velocidadY = 8f;
    [SerializeField] private InputActionAsset accionesInput;
    [SerializeField] private InputActionProperty accionMoverse;
    [SerializeField] private InputActionProperty accionSalto;
    [SerializeField] private InputActionProperty accionDisparo;
    [SerializeField] private InputActionProperty accionAcelerar;
    [SerializeField] TextMeshProUGUI panelPuntuacion;
    [SerializeField] TextMeshProUGUI tiempotranscurrido;
    [SerializeField] TextMeshProUGUI victoriaTxt;
    [SerializeField] Slider vidasSlider;
    [SerializeField] TextMeshProUGUI saltosTxt;
    [SerializeField] TextMeshProUGUI controlActivo;
    [SerializeField] GameObject gameOver;
    [SerializeField] GameObject gameUI;
    [SerializeField] GameObject menuPausa;
    [SerializeField] GameObject mensajeCuidado;
    
    public event Action DisparoSolicitado;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        accionesJugador = accionesInput.FindActionMap("Player", true);
        accionPausa = accionesJugador.FindAction("Pausa", true);
        accionSuperAtaque = accionesJugador.FindAction("SuperAtaque", true);
    }

    private void OnEnable()
    {
        accionesJugador.actionTriggered += ActualizarControlActivo;
        accionMoverse.action.performed += ActualizarMovimiento;
        accionMoverse.action.canceled += ActualizarMovimiento;
        accionSalto.action.performed += SolicitarSalto;
        accionDisparo.action.performed += SolicitarDisparo;
        accionPausa.performed += PausarJuego;
        accionSuperAtaque.started += IniciarCargaSuperAtaque;
        accionSuperAtaque.performed += EjecutarSuperAtaque;
        accionesJugador.Enable();
    }

    private void OnDisable()
    {
        accionesJugador.actionTriggered -= ActualizarControlActivo;
        accionMoverse.action.performed -= ActualizarMovimiento;
        accionMoverse.action.canceled -= ActualizarMovimiento;
        accionSalto.action.performed -= SolicitarSalto;
        accionDisparo.action.performed -= SolicitarDisparo;
        accionPausa.performed -= PausarJuego;
        accionSuperAtaque.started -= IniciarCargaSuperAtaque;
        accionSuperAtaque.performed -= EjecutarSuperAtaque;
        accionesJugador.Disable();
    }

    void Start()
    {
        vidas = 3;
        Time.timeScale=1f;
    }

    void Update()
    {
        //float horizontal = Input.GetAxisRaw("Horizontal")
        valorAceleracion = Mathf.Clamp01(accionAcelerar.action.ReadValue<float>());
        ActualizarTextoControlActivo();

        if (saltoSolicitado && tocandoSuelo)
        {
            rb.AddForce(new Vector2(0, velocidadY), ForceMode2D.Impulse);
            ultimoSalto = tiempoTotal;
            animator.SetBool("tocandoSuelo", false);
        }
        else if (saltoSolicitado && !tocandoSuelo && contadorSaltos != 0 && ultimoSalto < tiempoTotal - 0.5f)
        {
            rb.AddForce(new Vector2(0, velocidadY), ForceMode2D.Impulse);
            contadorSaltos--;
            saltosTxt.text = "Saltos: " + contadorSaltos;
            ultimoSalto = tiempoTotal;
            animator.SetBool("tocandoSuelo", false);
        }
        else
        {
            //Sprite Renderer y animator para Idle, moverse y saltar
            rb.AddForce(new Vector2(movimientoX * velocidadX * valorAceleracion, 0));
            if (movimientoX > 0)
            {
                spriteRenderer.flipX = false;
            }else if(movimientoX < 0)
            {
                spriteRenderer.flipX = true;
            }
            animator.SetFloat("andando", Mathf.Abs(movimientoX));
        }
        saltoSolicitado = false;

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

    private void ActualizarMovimiento(InputAction.CallbackContext context)
    {
        movimientoX = context.ReadValue<Vector2>().x;
    }

    private void SolicitarSalto(InputAction.CallbackContext context)
    {
        saltoSolicitado = true;
    }

    private void SolicitarDisparo(InputAction.CallbackContext context)
    {
        DisparoSolicitado?.Invoke();
    }

    private void ActualizarControlActivo(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed || context.control == null)
            return;

        string nombreControl;
        if (context.control.device is Gamepad)
            nombreControl = "GAMEPAD";
        else if (context.control.device is Keyboard || context.control.device is Mouse)
            nombreControl = "TECLADO Y RATON";
        else
            return;

        nombreControlActivo = nombreControl;
        ActualizarTextoControlActivo();
    }

    private void ActualizarTextoControlActivo()
    {
        string texto = $"control activo: \"{nombreControlActivo}\"\nAcelerar: {valorAceleracion:F2}";
        if (texto == textoControlActivoMostrado)
            return;

        textoControlActivoMostrado = texto;
        controlActivo.text = texto;
    }

    private void PausarJuego(InputAction.CallbackContext context)
    {
        Time.timeScale = 0f;
        menuPausa.SetActive(true);
    }

    private void IniciarCargaSuperAtaque(InputAction.CallbackContext context)
    {
        Debug.Log("Cargando superataque...");
    }

    private void EjecutarSuperAtaque(InputAction.CallbackContext context)
    {
        Debug.Log("¡SUPERATAQUE EJECUTADO!");
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
            animator.SetBool("tocandoSuelo", true);
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
            if (tieneLlave)
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

        if (collision.gameObject.CompareTag("MensajeCuidado"))
        {
            mensajeCuidado.SetActive(false);
        }
    }

    public void ReiniciarNivelActual()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
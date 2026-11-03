using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void JugarTutorial()
    {
        Time.timeScale=1f;
        SceneManager.LoadScene("primeraEscena2D");
    }

    public void JugarNivel1()
    {
        Time.timeScale=1f;
        SceneManager.LoadScene("segundaEscena2D");
    }

    public void VolverMenuInicio()
    {
        Time.timeScale=1f;
        SceneManager.LoadScene("PantallaInicio");
    }

    public void Salir()
    {
        Application.Quit();
    }
}

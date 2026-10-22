using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene(1);
        Time.timeScale=1f;
    }

    public void Salir()
    {
        Application.Quit();
    }
}

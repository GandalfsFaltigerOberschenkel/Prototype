using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void StartTheGame()
    {
        Destroy(UIManager.instance.gameObject);
        SceneManager.LoadScene("ScalingTest");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}

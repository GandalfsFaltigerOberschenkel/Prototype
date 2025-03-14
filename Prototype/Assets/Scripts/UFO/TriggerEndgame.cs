using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerEndgame : MonoBehaviour
{
    public TimerThing timerThing;
    public System.Diagnostics.Stopwatch endTimer;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            GameOver();
        }
    }
    public void GameOver()
    {
        timerThing.StopTimer();
        endTimer = timerThing.timer;
        PlayerPrefs.SetString("isAble", "1");
        SceneManager.LoadScene("EndCutScene");
    }
}

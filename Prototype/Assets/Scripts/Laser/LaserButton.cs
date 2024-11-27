using UnityEngine;

public class LaserButton : MonoBehaviour
{
    public Timer timer;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(timer.StartTimer());
        }
    }
}

using UnityEngine;

public class Laser : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider2D;
    public Timer timer;
    bool isEnabled = true;
    private void Awake()
    {
        timer.timerEnded += Toggle;
    }
    private void OnDestroy()
    {
        timer.timerEnded -= Toggle;
    }
    void Toggle()
    {
        if(isEnabled)
        {
            spriteRenderer.enabled = false;
            boxCollider2D.enabled = false;
            GetComponent<AudioSource>().Stop();
            isEnabled = false;
          
        }
        else
        {
            spriteRenderer.enabled = true;
            boxCollider2D.enabled = true;
            isEnabled = true;
            GetComponent<AudioSource>().Play();
            
        }
    }
}

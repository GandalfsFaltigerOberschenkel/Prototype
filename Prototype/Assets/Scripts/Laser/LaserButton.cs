using UnityEngine;

public class LaserButton : MonoBehaviour
{
    public Timer timer;
    public Sprite notPressedSprite;
    public Sprite pressedSprite;
    private bool isPressed = false;
    int callCount = 0;
    private void Start()
    {
        timer.timerEnded += OnTimerEnded;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isPressed)
        {
            PressButton();
        }
    }

    private void PressButton()
    {
        GetComponent<SpriteRenderer>().sprite = pressedSprite;
        isPressed = true;
        timer.isRepeating = true; // Allow one full cycle
        timer.StartTimer();
    }

    private void OnTimerEnded()
    {
        callCount++;
        // Reset button after one full cycle (active + inactive)
        if (isPressed && callCount % 2 == 0 )
        {
            
            GetComponent<SpriteRenderer>().sprite = notPressedSprite;
            isPressed = false;
        }
    }

    private void OnDestroy()
    {
        timer.timerEnded -= OnTimerEnded;
    }
}
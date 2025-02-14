using UnityEngine;

public class LaserButton : MonoBehaviour
{
    public Timer timer;
    bool isPressed = false;
    public Sprite notPressedSprite;
    public Sprite pressedSprite;
    int invokeCount = 0;
    private void Start()
    {
        timer.timerEnded += ResetButton;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!isPressed)
            {
                GetComponent<SpriteRenderer>().sprite = pressedSprite;
                isPressed = true;
                timer.StartCoroutine(timer.StartTimer());
            }
           
        }
    }
    private void ResetButton()
    {
        invokeCount++;
        if (invokeCount % 2 == 0)
        {
            GetComponent<SpriteRenderer>().sprite = notPressedSprite;
            isPressed = false;
        }
        
    }
}

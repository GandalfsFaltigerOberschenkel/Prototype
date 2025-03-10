using UnityEngine;

public class Laser : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer delayRenderer;
    public BoxCollider2D boxCollider2D;
    public Timer timer;
    private bool isEnabled = true;

    private void Awake()
    {
        // Initialize to default state
        SetLaserState(isEnabled);

        timer.timerEnded += Toggle;
        timer.timerAboutToEnd += ToggleRightBeforeEnded;
    }

    private void OnDestroy()
    {
        timer.timerEnded -= Toggle;
        timer.timerAboutToEnd -= ToggleRightBeforeEnded;
    }

    void ToggleRightBeforeEnded()
    {
        delayRenderer.enabled = true;
    }

    void Toggle()
    {
        isEnabled = !isEnabled;
        SetLaserState(isEnabled);
    }

    private void SetLaserState(bool enabled)
    {
        delayRenderer.enabled = enabled; // Always reset warning
        spriteRenderer.enabled = enabled;
        boxCollider2D.enabled = enabled;

        if (enabled)
            GetComponent<AudioSource>().Play();
        else
            GetComponent<AudioSource>().Stop();
    }
}
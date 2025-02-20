using UnityEngine;

public class Barometer : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;
    public Transform lowestPoint;
    public Transform highestPoint;
    public RectTransform background;
    public RectTransform bar;

    [Header("Settings")]
    [Tooltip("Offset for the bar's pivot point (0 = bottom, 1 = top)")]
    [Range(0, 1)] public float pivotOffset = 0f;

    private float minY;
    private float maxY;
    private float backgroundHeight;
    private Vector2 barPosition;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (!CheckReferences())
        {
            enabled = false;
            return;
        }

        UpdateYBounds();
        backgroundHeight = background.rect.height;
        bar.pivot = new Vector2(bar.pivot.x, pivotOffset);
    }

    private void Update()
    {
        UpdateBarPosition();
    }

    private void UpdateBarPosition()
    {
        // Update Y bounds in case they change during runtime
        UpdateYBounds();

        // Calculate normalized height position
        float playerY = playerTransform.position.y;
        float t = Mathf.InverseLerp(minY, maxY, playerY);

        // Calculate new bar position
        float newYPosition = t * backgroundHeight;
        barPosition = bar.anchoredPosition;
        barPosition.y = newYPosition;
        bar.anchoredPosition = barPosition;
    }

    private void UpdateYBounds()
    {
        minY = lowestPoint.position.y;
        maxY = highestPoint.position.y;
    }

    private bool CheckReferences()
    {
        bool isValid = true;

        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not assigned in Barometer!");
            isValid = false;
        }
        if (lowestPoint == null)
        {
            Debug.LogError("Lowest Point Transform is not assigned in Barometer!");
            isValid = false;
        }
        if (highestPoint == null)
        {
            Debug.LogError("Highest Point Transform is not assigned in Barometer!");
            isValid = false;
        }
        if (background == null)
        {
            Debug.LogError("Background RectTransform is not assigned in Barometer!");
            isValid = false;
        }
        if (bar == null)
        {
            Debug.LogError("Bar RectTransform is not assigned in Barometer!");
            isValid = false;
        }

        return isValid;
    }
}
using UnityEngine;

[System.Serializable]
public class ScrollableImage
{
    public GameObject image;
    public float scrollSpeed;
}

public class ParallexImageScroll : MonoBehaviour
{
    [SerializeField] ScrollableImage[] scrollableImages;
    public Vector2 scrollbounds; // X: min bound, Y: max bound
    public float midPoint = 0f; // Midpoint for initial positioning

    private Vector2 initialCamPos;
    private Vector2[] initialImagePositions;

    private void Start()
    {
        initialCamPos = Camera.main.transform.position - new Vector3(midPoint, 0f, 0f);
        scrollableImages = new ScrollableImage[transform.childCount];
        initialImagePositions = new Vector2[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            scrollableImages[i] = new ScrollableImage();
            scrollableImages[i].image = transform.GetChild(i).gameObject;

            // Ensure there's a ParallexImage component to get scrollSpeed from
            ParallexImage parallexImage = scrollableImages[i].image.GetComponent<ParallexImage>();
            if (parallexImage != null)
            {
                scrollableImages[i].scrollSpeed = parallexImage.scrollSpeed;
            }
            else
            {
                Debug.LogError("ParallexImage component missing on " + scrollableImages[i].image.name);
                scrollableImages[i].scrollSpeed = 0f;
            }

            // Set initial image positions relative to the midpoint
            initialImagePositions[i] = new Vector2(
                scrollableImages[i].image.transform.position.x,
                scrollableImages[i].image.transform.position.y
            );
        }
    }

    void Update()
    {
        Vector2 camPos = Camera.main.transform.position;
        Vector2 deltaCam = camPos - initialCamPos;

        for (int i = 0; i < scrollableImages.Length; i++)
        {
            // Calculate target position based on initial position and camera delta
            float targetX = initialImagePositions[i].x + deltaCam.x * scrollableImages[i].scrollSpeed;

            // Clamp the X position within the scroll bounds
            targetX = Mathf.Clamp(targetX, scrollbounds.x, scrollbounds.y);

            // Update the image's position
            Vector2 newPosition = new Vector2(
                targetX,
                scrollableImages[i].image.transform.position.y
            );
            scrollableImages[i].image.transform.position = newPosition;
        }
    }
}
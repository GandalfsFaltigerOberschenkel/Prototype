using UnityEngine;

public class RopeCollisionHandler : MonoBehaviour
{
    private bool isColliding;
    private RopeSystem ropeSystem;

    private void Awake()
    {
        ropeSystem = GetComponent<RopeSystem>();
        if (ropeSystem == null)
        {
            Debug.LogError("RopeCollisionHandler: Missing RopeSystem component.");
        }
    }

    private void OnTriggerStay2D(Collider2D colliderStay)
    {
        isColliding = true;
    }

    private void OnTriggerExit2D(Collider2D colliderOnExit)
    {
        isColliding = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle collision logic here if needed
    }

    public bool IsColliding()
    {
        return isColliding;
    }
}

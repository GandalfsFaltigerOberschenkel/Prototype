using UnityEngine;

public class HiddenPassage : MonoBehaviour
{
    public SpriteRenderer overlayLayer;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            overlayLayer.enabled = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            overlayLayer.enabled = true;
        }
    }
  
    
}

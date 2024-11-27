using System.Collections;
using UnityEngine;

public class FallThroughPlattforms : MonoBehaviour
{
    public Collider2D playerCollider;
    public LayerMask plattformMask;
    public float fallThroughTime = 0.5f;
    public PlayerController playerController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public IEnumerator FallThrough()
    {
        playerCollider.excludeLayers = plattformMask;
        playerController.isGrounded = false;
        yield return new WaitForSeconds(fallThroughTime);
        playerCollider.excludeLayers = 0;
        if (Physics2D.CapsuleCast(playerCollider.bounds.center, playerCollider.bounds.size, CapsuleDirection2D.Vertical, 0, Vector2.down, 0.1f, plattformMask))
        {
            StartCoroutine(FallThrough());
        }
        else
        {

        }
    }
}

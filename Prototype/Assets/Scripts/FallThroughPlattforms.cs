using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class FallThroughPlattforms : MonoBehaviour
{
    public Collider2D playerCollider;
    public LayerMask plattformMask;
    public float fallThroughTime = 0.5f;
    public PlayerController playerController;
    public CinemachineBasicMultiChannelPerlin cameraShake;

    private void Start()
    {
        cameraShake = FindAnyObjectByType<CinemachineBasicMultiChannelPerlin>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public IEnumerator FallThrough()
    {
        cameraShake.enabled = true;
        playerCollider.excludeLayers = plattformMask;
        playerController.isGrounded = false;
        yield return new WaitForSeconds(fallThroughTime);
        playerCollider.excludeLayers = 0;
        cameraShake.enabled = false;

        // Check if the player is still colliding with the platform
        while (Physics2D.CapsuleCast(playerCollider.bounds.center, playerCollider.bounds.size, CapsuleDirection2D.Vertical, 0, Vector2.down, 1f, plattformMask))
        {
            // Move the player slightly down to ensure they fall through
            transform.position += Vector3.down * 0.5f;
            yield return null;
           
        }
    }
}

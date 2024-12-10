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
        float timer = fallThroughTime;
        while (timer > 0)
        {
            playerController.GetComponent<RopeSystem>().ResetRope();
            timer -= Time.deltaTime;
            yield return null;
        }
        while (Physics2D.CapsuleCast(playerCollider.bounds.center, playerCollider.bounds.size, CapsuleDirection2D.Vertical, 0, Vector2.down, 1f, plattformMask))
        {
            playerController.GetComponent<RopeSystem>().ResetRope();
            yield return null;

        }
        playerCollider.excludeLayers = 0;
        cameraShake.enabled = false;

        // Check if the player is still colliding with the platform
        
    }
}

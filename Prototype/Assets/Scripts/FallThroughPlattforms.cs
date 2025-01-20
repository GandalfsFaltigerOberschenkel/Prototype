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
    public bool isFallingThrough = false;
    private void Start()
    {
        cameraShake = FindAnyObjectByType<CinemachineBasicMultiChannelPerlin>();
    }

    public IEnumerator FallThrough()
    {
        isFallingThrough = true;
        cameraShake.enabled = true;
        playerCollider.excludeLayers = plattformMask;
        playerController.isGrounded = false;
        float timer = fallThroughTime;

        while (timer > 0)
        {
            playerController.isGrounded = false;
            playerController.GetComponent<RopeSystem>().ResetRope();
            timer -= Time.deltaTime;
            yield return null;
        }

        while (Physics2D.CapsuleCast(playerCollider.bounds.center, playerCollider.bounds.size, CapsuleDirection2D.Vertical, 0, Vector2.down, 1f, plattformMask))
        {
            playerController.isGrounded = false;
            playerController.GetComponent<RopeSystem>().ResetRope();
            yield return null;
        }

        playerCollider.excludeLayers = 0;
        cameraShake.enabled = false;
        isFallingThrough = false;
        // Debugging-Ausgabe hinzufügen
        Debug.Log("FallThrough abgeschlossen. Spieler sollte nicht mehr durch Plattformen fallen.");
    }
    public void OnDrawGizmos()
    {
        //Draw Capsule Cast Gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(playerCollider.bounds.center, playerCollider.bounds.size);

    }
}

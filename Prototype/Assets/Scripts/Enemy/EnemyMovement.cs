using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 2f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void MoveDir(Vector2 direction)
    {
        rb.linearVelocity = direction * speed;
    }
}

using Pathfinding;
using UnityEngine;
using System.Collections;

public class DroneEnemy : EnemyController
{
    public AIDestinationSetter destinationSetter;
    public float hoverDistance = 2f;  // Distance for hovering near the player
    public float attackDiveSpeed = 5f;  // Speed for dive attack
    public float collisionTime = 1.5f; // Time the drone stays in attack mode after hitting the player
    // Distance to start attacking the player
    private Transform hoverTarget;
    private bool isHovering = false;
    private bool isAttacking = false;
    private float collisionTimer = 0f;
    private Collider2D droneCollider; // Reference to the drone's collider

    protected override void Start()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        droneCollider = GetComponent<Collider2D>(); // Get the collider component
        base.Start();
    }

    protected override void Update()
    {
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            destinationSetter.target = player.transform;
        }
        else
        {
            destinationSetter.target = null;
        }

        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdleState();
                break;
            case EnemyState.Walking:
                HandleWalkingState();
                break;
            case EnemyState.Attacking:
                HandleAttackingState();
                break;
            case EnemyState.TakeDamage:
                HandleTakeDamageState();
                break;
            case EnemyState.Dead:
                HandleDeadState();
                break;
        }
    }

    protected override void HandleIdleState()
    {
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            currentState = EnemyState.Attacking;
        }
        else
        {
            if (wayPoints.Length > 0)
                currentState = EnemyState.Walking;
        }
    }

    protected override void HandleWalkingState()
    {
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            currentState = EnemyState.Attacking;
        }
        else
        {
            base.HandleWalkingState();
        }
    }

    protected override void HandleAttackingState()
    {
        if (!isAttacking)
        {
            StartCoroutine(AttackAndReturnToHover());
        }
    }

    private IEnumerator AttackAndReturnToHover()
    {
        isAttacking = true;

        // Disable the collider during the attack


        // Start dive attack towards the player
        float attackTimer = collisionTime;
        while (attackTimer > 0)
        {
            // Move towards player at attack speed
            transform.position = Vector2.MoveTowards(transform.position, player.position, attackDiveSpeed * Time.deltaTime);
            attackTimer -= Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // After attack duration ends, move to a hover position
        ResetHoverPosition();

        // Re-enable the collider after the attack


        // Set the state back to idle to return to hovering
        currentState = EnemyState.Idle;  // Transition back to idle to hover again
        isAttacking = false;
    }

    private void ResetHoverPosition()
    {
        // Create a new hover position near the player after the attack
        hoverTarget = new GameObject("HoverTarget").transform;
        hoverTarget.position = player.position + new Vector3(Random.Range(-hoverDistance, hoverDistance), Random.Range(-hoverDistance, hoverDistance), 0);
        destinationSetter.target = hoverTarget;
    }

    protected override void HandleTakeDamageState()
    {
        base.HandleTakeDamageState();
    }

    protected override void HandleDeadState()
    {
        base.HandleDeadState();
    }

    protected override void Die()
    {
        base.Die();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {


        // If the drone collides with the player, start attacking
        if (collision.gameObject.CompareTag("Player"))
        {
            // Transition to attacking mode and immediately start the attack
            currentState = EnemyState.Attacking;
            StartCoroutine(collision.gameObject.GetComponent<FallThroughPlattforms>().FallThrough());
        }
    }
}

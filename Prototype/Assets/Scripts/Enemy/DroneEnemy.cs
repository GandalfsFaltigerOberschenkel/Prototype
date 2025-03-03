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
    private PolygonCollider2D droneCollider; // Reference to the drone's collider
    public Transform HomeBase;
    bool movingUp = true;
    bool isAirWiggling = false;

    protected override void Start()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        HomeBase = Instantiate(new GameObject("HomeBase"), transform.position, Quaternion.identity).transform;
        droneCollider = GetComponent<PolygonCollider2D>(); // Get the collider component
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
            case EnemyState.Reacovering:
                HandleRecoveringState();
                break;
        }
    }

    private void HandleRecoveringState()
    {
        if (currentState != EnemyState.Dead)
        {
            if (GetComponent<AIPath>().reachedDestination)
            {
                currentState = EnemyState.Idle;
            }
            else
            {
                destinationSetter.target = HomeBase;
            }

        }
    }

    protected override void HandleIdleState()
    {
        if (currentState == EnemyState.Dead)
        {
            return;
        }
        StartCoroutine(WiggleInAir());


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
    IEnumerator WiggleInAir()
    {
        if (isAirWiggling == false)
        {

            isAirWiggling = true;
            while (currentState == EnemyState.Idle)
            {
                if (movingUp)
                {
                    transform.position = new Vector2(transform.position.x, transform.position.y + 0.1f);
                    if (transform.position.y >= HomeBase.position.y + hoverDistance/3)
                    {
                        movingUp = false;
                    }
                }
                else
                {
                    transform.position = new Vector2(transform.position.x, transform.position.y - 0.1f);
                    if (transform.position.y <= HomeBase.position.y - hoverDistance/3)
                    {
                        movingUp = true;
                    }
                }
                yield return new WaitForSeconds(0.1f);
            }
            isAirWiggling = false;
        }
        else
        {
            yield return null;
        }
            
    }
    protected override void HandleWalkingState()
    {
        if (currentState == EnemyState.Dead)
        {
            return;
        }
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
        if (currentState == EnemyState.Dead)
        {
            return;
        }
        if (!isAttacking)
        {
            StartCoroutine(AttackAndReturnToHover());
        }
    }

    private IEnumerator AttackAndReturnToHover()
    {
        if (currentState == EnemyState.Dead)
        {
            yield return null;
        }
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
        currentState = EnemyState.Reacovering;  // Transition back to idle to hover again
        isAttacking = false;
    }

    private void ResetHoverPosition()
    {
        if (currentState == EnemyState.Dead)
        {
            return;
        }
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
        destinationSetter.enabled = false;
    }

    protected override void Die()
    {
        stunned = true;
        if (animator != null)
        {
            animator.SetBool("isDead", true);
        }
        droneCollider.enabled = false;
        if (GetComponent<Rigidbody2D>() != null)
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }
        currentState = EnemyState.Dead;

        StartCoroutine(FallDown());
        StartCoroutine(Stun());
    }

    protected override IEnumerator FallDown()
    {
        float elapsedTime = 0f;
        float fallDuration = 0.4f; // Dauer des Falls
        Vector2 initialPosition = transform.position;
        Vector2 targetPosition = new Vector2(transform.position.x, transform.position.y - droneCollider.bounds.size.y / 2);
        droneCollider.enabled = false;
        if (GetComponent<Rigidbody2D>())
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero; // Stop any existing velocity
        }
        while (elapsedTime < fallDuration)
        {
            transform.position = Vector2.Lerp(initialPosition, targetPosition, elapsedTime / fallDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
    protected override IEnumerator Stun()
    {
        yield return new WaitForSeconds(stunTime);

        stunned = false;
        if (animator != null)
            animator.SetBool("isDead", false);

        if (GetComponent<Rigidbody2D>() != null)
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;


        droneCollider.enabled = true;

        // Set the enemy back on top of the collider
        Vector2 targetPosition = new Vector2(transform.position.x, transform.position.y + droneCollider.bounds.size.y / 2);
        transform.position = targetPosition;
        currentState = EnemyState.Idle;

    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {


        // If the drone collides with the player, start attacking
        if (collision.gameObject.CompareTag("Player"))
        {
            if (currentState != EnemyState.Dead)
            {
                // Transition to attacking mode and immediately start the attack
                currentState = EnemyState.Attacking;
                StartCoroutine(collision.gameObject.GetComponent<FallThroughPlattforms>().FallThrough());
            }
        }
    }
}

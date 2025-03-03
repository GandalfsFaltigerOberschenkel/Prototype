using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EnemyState
{
    Idle,
    Walking,
    Attacking,
    TakeDamage,
    Reacovering,
    Dead
}
public abstract class EnemyController : MonoBehaviour
{
    public int health = 100;
    public EnemyMovement movement;
    public Transform[] wayPoints;
    public float attackRange = 1f;
    public Transform player;
    protected int currentWayPointIndex = 0;
    public EnemyState currentState = EnemyState.Idle;
    protected Animator animator;
    public float pushStrength = 5f;
    public float idleTime = 2f; // Zeit im Leerlauf
    public delegate void EnemyDestroyedHandler(EnemyController enemy);
    public event EnemyDestroyedHandler OnEnemyDestroyed;
    public float stunTime = 5f;
    public bool stunned = false;
    private Coroutine idleCoroutine;



    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        idleCoroutine = StartCoroutine(IdleBeforeNextWaypoint());
    }

    protected virtual void Update()
    {
        Debug.Log($"Update - Current State: {currentState}"); // Add this line

        if (currentState == EnemyState.Dead) return; // Exit early if dead

        switch (currentState)
        {
            case EnemyState.Idle:
                Debug.Log("Handling Idle State");  // Add this
                HandleIdleState();
                break;
            case EnemyState.Walking:
                Debug.Log("Handling Walking State");  // Add this
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

    public virtual void TakeDamage(int damage)
    {
        if (currentState == EnemyState.Dead) return;

        health -= damage;
        if (health <= 0)
        {
            Die();
            currentState = EnemyState.Dead;
        }
        else
        {
            currentState = EnemyState.TakeDamage;
        }
    }

    protected virtual void Die()
    {
        if (idleCoroutine != null) StopCoroutine(idleCoroutine);

        stunned = true;
        if (animator != null)
        {
            animator.SetBool("isDead", true);
        }
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (GetComponent<Rigidbody2D>() != null)
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }
        currentState = EnemyState.Dead;

        StartCoroutine(FallDown());
        StartCoroutine(Stun());
    }

    protected virtual IEnumerator FallDown()
    {
        float elapsedTime = 0f;
        float fallDuration = 0.4f; // Dauer des Falls
        Vector2 initialPosition = transform.position;
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Vector2 targetPosition = new Vector2(transform.position.x, transform.position.y - collider.bounds.size.y / 2);
        collider.enabled = false;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero; // Stop any existing velocity

        while (elapsedTime < fallDuration)
        {
            transform.position = Vector2.Lerp(initialPosition, targetPosition, elapsedTime / fallDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }

    protected virtual IEnumerator Stun()
    {
        yield return new WaitForSeconds(stunTime);

        stunned = false;
        animator.SetBool("isDead", false);
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.enabled = true;

        // Set the enemy back on top of the collider
        Vector2 targetPosition = new Vector2(transform.position.x, transform.position.y + collider.bounds.size.y / 2);
        transform.position = targetPosition;

        StartCoroutine(IdleBeforeNextWaypoint());
    }

    protected virtual void HandleIdleState()
    {
        if (stunned) return; // Exit if stunned

        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            currentState = EnemyState.Attacking;
        }
    }

    protected virtual void HandleWalkingState()
    {
        Debug.Log($"HandleWalkingState called. Distance to waypoint: {Vector2.Distance(transform.position, wayPoints[currentWayPointIndex].position)}");

        if (currentState == EnemyState.Dead || stunned) return;

        if (Vector2.Distance(transform.position, wayPoints[currentWayPointIndex].position) > 0.5f)
        {
            Vector2 currentWayPointPos = wayPoints[currentWayPointIndex].position;
            Vector2 wayPointDir = currentWayPointPos - (Vector2)transform.position;
            Debug.Log($"Moving towards waypoint {currentWayPointIndex}. Direction: {wayPointDir}");
            movement.MoveDir(wayPointDir.normalized);

            // Flip the sprite based on the movement direction
            if (wayPointDir.x > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (wayPointDir.x < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            StartCoroutine(IdleBeforeNextWaypoint());
        }

        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            currentState = EnemyState.Attacking;
        }
    }

    private IEnumerator IdleBeforeNextWaypoint()
    {
        Debug.Log("Starting idle period");
        currentState = EnemyState.Idle;
        yield return new WaitForSeconds(idleTime);

        if (wayPoints.Length > 0)
        {
            Debug.Log($"Transitioning to walking state. Current waypoint index: {currentWayPointIndex}");
            currentWayPointIndex++;
            currentWayPointIndex = currentWayPointIndex % wayPoints.Length;
            currentState = EnemyState.Walking;
        }
    }

    protected abstract void HandleAttackingState();

    protected virtual void HandleTakeDamageState()
    {
        currentState = EnemyState.Idle;
    }

    protected virtual void HandleDeadState()
    {

    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            currentState = EnemyState.Attacking;
        }
    }
}

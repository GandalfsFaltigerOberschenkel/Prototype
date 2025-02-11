using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EnemyState
{
    Idle,
    Walking,
    Attacking,
    TakeDamage,
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

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        StartCoroutine(IdleBeforeNextWaypoint());
    }

    protected virtual void Update()
    {
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
        if (OnEnemyDestroyed != null)
        {
            OnEnemyDestroyed(this);
        }
        GetComponent<Animator>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        movement.enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    protected virtual void HandleIdleState()
    {
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            currentState = EnemyState.Attacking;
        }
      
    }

    protected virtual void HandleWalkingState()
    {
        if (Vector2.Distance(transform.position, wayPoints[currentWayPointIndex].position) > 0.5f)
        {
            Vector2 currentWayPointPos = wayPoints[currentWayPointIndex].position;
            Vector2 wayPointDir = currentWayPointPos - (Vector2)transform.position;
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
        currentState = EnemyState.Idle;
        yield return new WaitForSeconds(idleTime);
        currentWayPointIndex++;
        currentWayPointIndex = currentWayPointIndex % wayPoints.Length;
        currentState = EnemyState.Walking;
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

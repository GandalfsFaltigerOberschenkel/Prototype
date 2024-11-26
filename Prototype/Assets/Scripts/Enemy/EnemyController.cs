using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Walking,
        Attacking,
        TakeDamage,
        Dead
    }

    public int health = 100;
    public EnemyMovement movement;
    public Transform[] wayPoints;
    public float attackRange = 1f;
    public Transform player;
    private int currentWayPointIndex = 0;
    private EnemyState currentState = EnemyState.Idle;
    private Animator animator;
    public delegate void EnemyDestroyedHandler(EnemyController enemy);
    public event EnemyDestroyedHandler OnEnemyDestroyed;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
    }

    void Update()
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

        UpdateAnimator();
    }

    public void TakeDamage(int damage)
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
    private void Die()
    {
        // Lösen Sie das Ereignis aus, wenn der Feind stirbt
        if (OnEnemyDestroyed != null)
        {
            OnEnemyDestroyed(this);
        }
        GetComponent<Animator>().enabled = false;  
        GetComponent<Collider2D>().enabled = false;
        movement.enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private void HandleIdleState()
    {
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            currentState = EnemyState.Attacking;
        }
        else
        {
            currentState = EnemyState.Walking;
        }
    }

    private void HandleWalkingState()
    {
        if (Vector2.Distance(transform.position, wayPoints[currentWayPointIndex].position) > 0.5f)
        {
            Vector2 currentWayPointPos = wayPoints[currentWayPointIndex].position;
            Vector2 wayPointDir = currentWayPointPos - (Vector2)transform.position;
            movement.MoveDir(wayPointDir.normalized);
        }
        else
        {
            currentWayPointIndex++;
            currentWayPointIndex = currentWayPointIndex % wayPoints.Length;
            currentState = EnemyState.Idle;
        }

        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            currentState = EnemyState.Attacking;
        }
    }

    private void HandleAttackingState()
    {
        movement.MoveDir(Vector2.zero);

        if (Vector2.Distance(transform.position, player.position) > attackRange)
        {
            currentState = EnemyState.Walking;
        }
    }

    private void HandleTakeDamageState()
    {
       
        currentState = EnemyState.Idle; 
    }

    private void HandleDeadState()
    {
       
    }

    private void UpdateAnimator()
    {
        animator.SetBool("isIdle", currentState == EnemyState.Idle);
        animator.SetBool("isWalking", currentState == EnemyState.Walking);
        animator.SetBool("isAttacking", currentState == EnemyState.Attacking);
        animator.SetBool("isTakeDamage", currentState == EnemyState.TakeDamage);
        animator.SetBool("isDead", currentState == EnemyState.Dead);
    }
}

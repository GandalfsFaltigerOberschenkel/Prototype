using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Walking,
        Attacking
    }

    public EnemyMovement movement;
    public Transform[] wayPoints;
    public float attackRange = 1f;
    public Transform player;
    private int currentWayPointIndex = 0;
    private EnemyState currentState = EnemyState.Idle;
    public float wayPointReachedDistance = 0.25f;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
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
        }
    }

    private void HandleIdleState()
    {
        // Logik für den Idle-Zustand
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
        if (Vector2.Distance(transform.position, wayPoints[currentWayPointIndex].position) > wayPointReachedDistance)
        {
            Vector2 currentWayPointPos = wayPoints[currentWayPointIndex].position;
            Vector2 wayPointDir = currentWayPointPos - (Vector2)transform.position;
            Debug.Log("Moving towards waypoint: " + currentWayPointIndex);
            movement.MoveDir(wayPointDir.normalized);
        }
        else
        {
            Debug.Log("Reached waypoint: " + currentWayPointIndex);
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
        // Logik für den Angriffs-Zustand
        Debug.Log("Attacking player!");
        movement.MoveDir(Vector2.zero); // Stoppt die Bewegung

        if (Vector2.Distance(transform.position, player.position) > attackRange)
        {
            currentState = EnemyState.Walking;
        }
    }
}

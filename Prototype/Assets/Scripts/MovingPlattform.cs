using System.Collections;
using UnityEngine;

public class MovingPlattform : MonoBehaviour
{
   public Transform[] waypoints;
    public float speed = 1;
    public bool cyclic;
    public float waitTime;
    
    public float easeAmount;
    int currentWaypointIndex = 0;

    private void Start()
    {
       transform.position = waypoints[currentWaypointIndex].position;
    }
    private void Update()
    {
        if(Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.02f)
        {
            if(waitTime > 0)
            {
                StartCoroutine(Wait());
            }
            else
            {
                ChangeWaypoint();
            }
        }
        Move();
    }
    private void Move()
    {
        Vector2 moveDir = waypoints[currentWaypointIndex].position - transform.position;
        float dist = moveDir.magnitude;
        float easedDist = Mathf.Pow(dist, easeAmount); // Adjust the easing function
        float moveAmount = Mathf.Clamp01(easedDist * speed * Time.deltaTime);
        Vector3 move = moveDir.normalized * moveAmount;
        transform.position += move;
    }
    
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
        ChangeWaypoint();
    }
    void ChangeWaypoint()
    {
        if(cyclic)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
        else
        {
            if(currentWaypointIndex + 1 < waypoints.Length)
            {
                currentWaypointIndex++;
            }
        }
    }
}

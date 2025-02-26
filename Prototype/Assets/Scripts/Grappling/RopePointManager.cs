using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class RopePointManager : MonoBehaviour
{
    public List<Transform> ropePoints = new List<Transform>();
    public Dictionary<Transform, int> wrapPointsLookup = new Dictionary<Transform, int>();
    public bool distanceSet;
    private Rigidbody2D ropeHingeAnchorRb;
    private LineRenderer ropeRenderer;
    private DistanceJoint2D ropeJoint;
    private SpriteRenderer ropeHingeAnchorSprite;
    private Transform playerTransform;
    private EnemyController currentEnemy;
    public Dictionary<Transform, Vector2> collisionNormals = new Dictionary<Transform, Vector2>();

    public void Initialize(Rigidbody2D ropeHingeAnchorRb, LineRenderer ropeRenderer, DistanceJoint2D ropeJoint, SpriteRenderer ropeHingeAnchorSprite, Transform playerTransform)
    {
        this.ropeHingeAnchorRb = ropeHingeAnchorRb;
        this.ropeRenderer = ropeRenderer;
        this.ropeJoint = ropeJoint;
        this.ropeHingeAnchorSprite = ropeHingeAnchorSprite;
        this.playerTransform = playerTransform;
    }

    public void AddRopePoint(Vector2 position, Vector2 normal, Transform parent)
    {
        var newRopePoint = new GameObject("RopePoint").transform;
        newRopePoint.position = position;
        newRopePoint.SetParent(parent);
        ropePoints.Add(newRopePoint);
        collisionNormals.Add(newRopePoint, normal);
    }
   
    public void RemoveLastRopePoint()
    {
        if (ropePoints.Count > 0)
        {
            var lastRopePoint = ropePoints.Last();
            Destroy(lastRopePoint.gameObject);
            ropePoints.RemoveAt(ropePoints.Count - 1);
            wrapPointsLookup.Remove(lastRopePoint);
            distanceSet = false;
        }
    }
    private Vector2 GetCollisionNormal(Transform ropePoint)
    {
        return collisionNormals.TryGetValue(ropePoint, out Vector2 normal) ?
               normal :
               Vector2.up; // Fallback if normal not found
    }
    public void UpdateRopePositions()
    {
        if (ropePoints.Count == 0) return;

        ropeRenderer.positionCount = ropePoints.Count + 1;

        for (var i = ropeRenderer.positionCount - 1; i >= 0; i--)
        {
            if (i != ropeRenderer.positionCount - 1)
            {
                ropeRenderer.SetPosition(i, ropePoints[i].position);

                if (i == ropePoints.Count - 1 || ropePoints.Count == 1)
                {
                    // Get the LAST rope point's position
                    var ropePosition = ropePoints[0].position;
                    ropeHingeAnchorRb.transform.position = ropePosition;

                    if (ropeHingeAnchorSprite != null)
                    {
                        ropeHingeAnchorSprite.transform.position = ropePosition;

                        // Get collision normal from your data structure
                        Vector2 normal = GetCollisionNormal(ropePoints[0]);

                        // Calculate tangent direction (parallel to wall)
                        Vector2 tangent = new Vector2(normal.y, -normal.x).normalized;

                        // Calculate rotation angle
                        float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg-90;
                        ropeHingeAnchorSprite.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    }

                    if (!distanceSet)
                    {
                        ropeJoint.distance = Vector2.Distance(playerTransform.position, ropePosition);
                        distanceSet = true;
                    }
                }
            }
            else
            {
                ropeRenderer.SetPosition(i, playerTransform.position);
            }
        }

        if (currentEnemy != null)
        {
            ropeRenderer.SetPosition(ropeRenderer.positionCount - 1, currentEnemy.transform.position);
        }
    }

    public void SetCurrentEnemy(EnemyController enemy)
    {
        currentEnemy = enemy;
    }

    public void ClearRopePoints()
    {
        foreach (var point in ropePoints)
        {
            Destroy(point.gameObject);
        }
        ropePoints.Clear();
        wrapPointsLookup.Clear();
        distanceSet = false;
    }

    public bool ContainsPoint(Transform position)
    {
        return wrapPointsLookup.ContainsKey(position);
    }

    public Transform GetLastRopePoint()
    {
        return ropePoints.Count > 0 ? ropePoints.Last() : null;
    }

    public int GetRopePointCount()
    {
        return ropePoints.Count;
    }
    public bool RemoveInvalidRopePoints()
    {
        bool removed = false;
        for (int i = ropePoints.Count - 1; i >= 0; i--)
        {
            if (ropePoints[i] == null || ropePoints[i].parent == null)
            {
                var invalidRopePoint = ropePoints[i];
                ropePoints.RemoveAt(i);
                wrapPointsLookup.Remove(invalidRopePoint);
                distanceSet = false;
                removed = true;
            }
        }
        return removed;
    }
}

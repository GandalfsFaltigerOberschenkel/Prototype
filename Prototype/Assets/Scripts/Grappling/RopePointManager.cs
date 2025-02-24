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

    public void Initialize(Rigidbody2D ropeHingeAnchorRb, LineRenderer ropeRenderer, DistanceJoint2D ropeJoint, SpriteRenderer ropeHingeAnchorSprite, Transform playerTransform)
    {
        this.ropeHingeAnchorRb = ropeHingeAnchorRb;
        this.ropeRenderer = ropeRenderer;
        this.ropeJoint = ropeJoint;
        this.ropeHingeAnchorSprite = ropeHingeAnchorSprite;
        this.playerTransform = playerTransform;
    }

    public void AddRopePoint(Vector2 position, Transform parent)
    {
        var newRopePoint = new GameObject("RopePoint").transform;
        newRopePoint.position = position;
        newRopePoint.SetParent(parent);
        ropePoints.Add(newRopePoint);
        wrapPointsLookup.Add(newRopePoint, 0);
        distanceSet = false;
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
                    var ropePosition = ropePoints[0].position;
                    ropeHingeAnchorRb.transform.position = ropePosition;
                    if (ropeHingeAnchorSprite != null)
                    {
                        ropeHingeAnchorSprite.transform.position = ropePosition;
                        Vector2 dir ;
                        if (ropePoints.Count == 1)
                        {
                            dir = ropePosition - playerTransform.position;
                            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                            ropeHingeAnchorSprite.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                        }
                        else
                        {
                            ropePosition = ropePoints[1].position;
                            //dir = ropePosition - ropePoints[0].position;
                        }

                        // Calculate the rotation angle and adjust by -90 degrees
                        

                        //// Mirror the sprite if shooting to the left
                        //Vector3 scale = ropeHingeAnchorSprite.transform.localScale;
                        //scale.x = Mathf.Sign(dir.x) * Mathf.Abs(scale.x);
                        //ropeHingeAnchorSprite.transform.localScale = scale;
                    }
                    else
                    {
                        Debug.LogWarning("RopeHingeAnchorSprite is not set!");
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

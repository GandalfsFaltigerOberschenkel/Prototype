using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class RopeSystem : MonoBehaviour
{
    public GameObject ropeHingeAnchor;
    public DistanceJoint2D ropeJoint;
    public Transform crosshair;
    public SpriteRenderer crosshairSprite;
    public PlayerMovement playerMovement;
    private bool ropeAttached;
    private Vector2 playerPosition;
    private Rigidbody2D ropeHingeAnchorRb;
    private SpriteRenderer ropeHingeAnchorSprite;
    public LineRenderer ropeRenderer;
    public LayerMask ropeLayerMask;
    private float ropeMaxCastDistance = 5f;
    private List<Transform> ropePoints = new List<Transform>();
    private bool distanceSet;
    private Dictionary<Transform, int> wrapPointsLookup = new Dictionary<Transform, int>();
    public float climbSpeed = 3f;
    private bool isColliding;
    public float pushForce = 10f;
    public PlayerController player;
    public LayerMask destroyRopeMask;
    public GameObject[] ropeTrys;
    private int remainingRopeTries;

    private void Awake()
    {
        ropeJoint.enabled = false;
        playerPosition = transform.position;
        ropeHingeAnchorRb = ropeHingeAnchor.GetComponent<Rigidbody2D>();
        ropeHingeAnchorSprite = ropeHingeAnchor.GetComponent<SpriteRenderer>();
        remainingRopeTries = ropeTrys.Length;
        UpdateRopeTrys();
    }

    private void Update()
    {
        if (playerMovement.isGround && !playerMovement.isSwinging)
        {
            remainingRopeTries = ropeTrys.Length;
            UpdateRopeTrys();
        }
        var aimDirection = player.input.aimDirection;
        var aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x);
        playerPosition = transform.position;

        if (!ropeAttached)
        {
            SetCrosshairPosition(aimAngle);
            playerMovement.isSwinging = false;
        }
        else
        {
            playerMovement.isSwinging = true;
            playerMovement.ropeHook = ropePoints.Last().position;
            crosshairSprite.enabled = false;

            if (ropePoints.Count > 0)
            {
                var lastRopePoint = ropePoints.Last();
                var playerToCurrentNextHit = Physics2D.Raycast(playerPosition, ((Vector2)lastRopePoint.position - playerPosition).normalized, Vector2.Distance(playerPosition, lastRopePoint.position) - 0.1f, ropeLayerMask);
                var playerCheckIfHitDestroyRope = Physics2D.Raycast(playerPosition, ((Vector2)lastRopePoint.position - playerPosition).normalized, Vector2.Distance(playerPosition, lastRopePoint.position) - 0.1f, destroyRopeMask);
                if (playerCheckIfHitDestroyRope)
                {
                    ResetRope();
                    return;
                }
                if (playerToCurrentNextHit)
                {
                    var colliderWithVertices = playerToCurrentNextHit.collider as PolygonCollider2D;
                    if (colliderWithVertices != null)
                    {
                        var closestPointToHit = GetClosestColliderPointFromRaycastHit(playerToCurrentNextHit, colliderWithVertices);

                        if (wrapPointsLookup.ContainsKey(closestPointToHit))
                        {
                            ResetRope();
                            return;
                        }

                        var newRopePoint = new GameObject("RopePoint").transform;
                        newRopePoint.position = closestPointToHit.position;
                        newRopePoint.SetParent(playerToCurrentNextHit.collider.transform);
                        ropePoints.Add(newRopePoint);
                        wrapPointsLookup.Add(newRopePoint, 0);
                        distanceSet = false;
                    }
                }
            }
        }

        HandleInput(aimDirection);
        UpdateRopePositions();

        if (ropePoints.Count > 1)
        {
            UnwrapRopeSegment();
        }

        HandleRopeLength();
    }

    private void SetCrosshairPosition(float aimAngle)
    {
        if (!crosshairSprite.enabled)
        {
            crosshairSprite.enabled = true;
        }

        var x = transform.position.x + 0.25f * Mathf.Cos(aimAngle);
        var y = transform.position.y + 0.25f * Mathf.Sin(aimAngle);

        var crossHairPosition = new Vector3(x, y, 0);
        crosshair.transform.position = crossHairPosition;
    }

    private void HandleInput(Vector2 aimDirection)
    {
        if (player.input.swingButtonHeld)
        {
            if (ropeAttached || remainingRopeTries <= 0) return;
            ropeRenderer.enabled = true;

            var hit = Physics2D.Raycast(playerPosition, aimDirection, ropeMaxCastDistance, ropeLayerMask);
            var checkDestroyRopeObj = Physics2D.Raycast(playerPosition, aimDirection, ropeMaxCastDistance, destroyRopeMask);
            if (checkDestroyRopeObj)
            {
                ResetRope();
                return;
            }
            if (hit.collider != null)
            {
                ropeAttached = true;
                if (!ropePoints.Any(p => p.position == (Vector3)hit.point))
                {
                    // We hit a new point, add that to our rope points
                    transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, pushForce), ForceMode2D.Impulse); // Push the player away from the ground
                    var newRopePoint = new GameObject("RopePoint").transform;
                    newRopePoint.position = hit.point;
                    newRopePoint.SetParent(hit.collider.transform);
                    ropePoints.Add(newRopePoint);
                    ropeJoint.distance = Vector2.Distance(playerPosition, hit.point);
                    ropeJoint.enabled = true;
                    ropeHingeAnchorSprite.enabled = true;
                    remainingRopeTries--;
                    UpdateRopeTrys();
                }
            }
            else
            {
                ropeRenderer.enabled = false;
                ropeAttached = false;
                ropeJoint.enabled = false;
            }
        }

        if (player.input.swingButtonReleased)
        {
            ResetRope();
        }
    }

    private void ResetRope()
    {
        ropeJoint.enabled = false;
        ropeAttached = false;
        playerMovement.isSwinging = false;
        ropeRenderer.positionCount = 2;
        ropeRenderer.SetPosition(0, transform.position);
        ropeRenderer.SetPosition(1, transform.position);
        foreach (var ropePoint in ropePoints)
        {
            Destroy(ropePoint.gameObject);
        }
        ropePoints.Clear();
        ropeHingeAnchorSprite.enabled = false;
        wrapPointsLookup.Clear();
    }

    private void UpdateRopePositions()
    {
        if (!ropeAttached) return;

        ropeRenderer.positionCount = ropePoints.Count + 1;

        for (var i = ropeRenderer.positionCount - 1; i >= 0; i--)
        {
            if (i != ropeRenderer.positionCount - 1)
            {
                ropeRenderer.SetPosition(i, ropePoints[i].position);

                if (i == ropePoints.Count - 1 || ropePoints.Count == 1)
                {
                    var ropePosition = ropePoints[ropePoints.Count - 1].position;
                    ropeHingeAnchorRb.transform.position = ropePosition;
                    if (!distanceSet)
                    {
                        ropeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                        distanceSet = true;
                    }
                }
            }
            else
            {
                ropeRenderer.SetPosition(i, transform.position);
            }
        }
    }

    private void UnwrapRopeSegment()
    {
        var lastWrapPoint = ropePoints.Last();
        var distanceToLastPoint = Vector2.Distance(playerPosition, lastWrapPoint.position);

        float unwrapDistanceThreshold = 0.5f;

        if (distanceToLastPoint < unwrapDistanceThreshold)
        {
            Destroy(lastWrapPoint.gameObject);
            ropePoints.RemoveAt(ropePoints.Count - 1);
            wrapPointsLookup.Remove(lastWrapPoint);
            distanceSet = false;
        }
    }

    private Transform GetClosestColliderPointFromRaycastHit(RaycastHit2D hit, PolygonCollider2D polyCollider)
    {
        var distanceDictionary = polyCollider.points.ToDictionary<Vector2, float, Vector2>(
            position => Vector2.Distance(hit.point, polyCollider.transform.TransformPoint(position)),
            position => polyCollider.transform.TransformPoint(position));

        var orderedDictionary = distanceDictionary.OrderBy(e => e.Key);
        var closestPoint = orderedDictionary.Any() ? orderedDictionary.First().Value : Vector2.zero;

        // Create a new GameObject at the closest point and set its parent to the hit collider's transform
        var newRopePoint = new GameObject("RopePoint").transform;
        newRopePoint.position = closestPoint;
        newRopePoint.SetParent(hit.collider.transform);

        return newRopePoint;
    }

    private void HandleRopeLength()
    {
        if (ropeAttached && !isColliding)
        {
            ropeJoint.distance -= Time.deltaTime * climbSpeed;
        }
    }

    void OnTriggerStay2D(Collider2D colliderStay)
    {
        isColliding = true;
    }

    private void OnTriggerExit2D(Collider2D colliderOnExit)
    {
        isColliding = false;
    }

    private void UpdateRopeTrys()
    {
        for (int i = 0; i < ropeTrys.Length; i++)
        {
            ropeTrys[i].SetActive(i < remainingRopeTries);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}

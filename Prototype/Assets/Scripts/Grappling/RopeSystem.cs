using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class RopeSystem : MonoBehaviour
{
    public GameObject ropeHingeAnchor;
    public Transform crosshair;
    public SpriteRenderer crosshairSprite;
    public PlayerMovement playerMovement;
    private Vector2 playerPosition;
    public Rigidbody2D ropeHingeAnchorRb;
    public SpriteRenderer ropeHingeAnchorSprite;
    public LineRenderer ropeRenderer;
    public LayerMask ropeLayerMask;
    public LayerMask coinLayerMask; // Neue LayerMask f�r Coin
    public float ropeMaxCastDistance = 5f;
    public float climbSpeed = 3f;
    public float pushForce = 10f;
    public PlayerController player;
    public LayerMask destroyRopeMask;
    public int damage = 50;
    public RopeStateManager ropeStateManager;
    public RopePointManager ropePointManager;
    private RopeCollisionHandler ropeCollisionHandler;
    private EnemyInteractionHandler enemyInteractionHandler;
    private RopeCoinCollector ropeCoinCollector;
    public DistanceJoint2D ropeJoint;
    public GameObject[] ropeTrys;

    private void Awake()
    {
        if (ropeHingeAnchor == null || ropeRenderer == null || ropeJoint == null)
        {
            Debug.LogError("RopeSystem: Missing required components.");
            return;
        }
        ropeCoinCollector = GetComponent<RopeCoinCollector>();
        if (ropeCoinCollector == null)
        {
            Debug.LogError("RopeSystem: Missing RopeCoinCollector component.");
        }
        ropeCollisionHandler = GetComponent<RopeCollisionHandler>();
        if (ropeCollisionHandler == null)
        {
            Debug.LogError("RopeSystem: Missing RopeCollisionHandler component.");
        }
        enemyInteractionHandler = GetComponent<EnemyInteractionHandler>();
        if (enemyInteractionHandler == null)
        {
            Debug.LogError("RopeSystem: Missing EnemyInteractionHandler component.");
        }

        ropeJoint.enabled = false;
        playerPosition = transform.position;
        ropeHingeAnchorRb = ropeHingeAnchor.GetComponent<Rigidbody2D>();
        ropeHingeAnchorSprite = ropeHingeAnchor.GetComponent<SpriteRenderer>();

        ropeStateManager.Initialize(ropeTrys, playerMovement, ropeRenderer, ropeJoint, ropeHingeAnchorSprite, ropePointManager);
        ropePointManager.Initialize(ropeHingeAnchorRb, ropeRenderer, ropeJoint, ropeHingeAnchorSprite, transform);
    }

    private void Update()
    {
        if (playerMovement.isGround && !playerMovement.isSwinging)
        {
            ropeStateManager.UpdateRopeTrys();
            ropeStateManager.ResetRopeTrys();
        }

        var aimDirection = player.input.aimDirection;
        var aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x);
        playerPosition = transform.position;

        if (!ropeStateManager.IsRopeAttached())
        {
            SetCrosshairPosition(aimAngle);
            playerMovement.isSwinging = false;
        }
        else
        {
            HandleSwinging();
        }

        HandleRopeLength();
        ropePointManager.UpdateRopePositions();

        if (ropePointManager.GetRopePointCount() > 1)
        {
            UnwrapRopeSegment();
        }
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

    private void HandleSwinging()
    {
        playerMovement.isSwinging = true;
        if (ropePointManager.GetRopePointCount() > 0)
        {
            playerMovement.ropeHook = ropePointManager.GetLastRopePoint().position;
        }
        crosshairSprite.enabled = false;

        if (ropePointManager.GetRopePointCount() > 0)
        {
            var lastRopePoint = ropePointManager.GetLastRopePoint();
            var playerToCurrentNextHit = Physics2D.Raycast(playerPosition, ((Vector2)lastRopePoint.position - playerPosition).normalized, Vector2.Distance(playerPosition, lastRopePoint.position) - 0.1f, ropeLayerMask);
            var playerCheckIfHitDestroyRope = Physics2D.Raycast(playerPosition, ((Vector2)lastRopePoint.position - playerPosition).normalized, Vector2.Distance(playerPosition, lastRopePoint.position) - 0.1f, destroyRopeMask);
            if (playerCheckIfHitDestroyRope)
            {
                ropeStateManager.ResetRope();
                return;
            }
            if (playerToCurrentNextHit)
            {
                var colliderWithVertices = playerToCurrentNextHit.collider as PolygonCollider2D;
                if (colliderWithVertices != null)
                {
                    var closestPointToHit = GetClosestColliderPointFromRaycastHit(playerToCurrentNextHit, colliderWithVertices);

                    if (ropePointManager.ContainsPoint(closestPointToHit.position))
                    {
                        ropeStateManager.ResetRope();
                        return;
                    }

                    ropePointManager.AddRopePoint(closestPointToHit.position, playerToCurrentNextHit.collider.transform);
                }
            }
        }
    }

    private void UnwrapRopeSegment()
    {
        var lastWrapPoint = ropePointManager.GetLastRopePoint();
        var distanceToLastPoint = Vector2.Distance(playerPosition, lastWrapPoint.position);

        Destroy(lastWrapPoint.gameObject);
        ropePointManager.RemoveLastRopePoint();
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
        if (ropeStateManager.IsRopeAttached())
        {
            ropeJoint.distance -= climbSpeed * Time.deltaTime;
        }
    }
}

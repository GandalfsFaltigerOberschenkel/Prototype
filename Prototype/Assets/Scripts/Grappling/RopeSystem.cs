using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

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
    public LayerMask coinLayerMask;
    public float ropeMaxCastDistance = 5f;
    public float climbSpeed = 3f;
    public float pushForce = 10f;
    public PlayerController player;
    public LayerMask destroyRopeMask;
    public int damage = 50;
    public RopeStateManager ropeStateManager;
    public RopePointManager ropePointManager;
    public RopeCoinCollector ropeCoinCollector;
    private RopeCollisionHandler ropeCollisionHandler;
    private EnemyInteractionHandler enemyInteractionHandler;
    public DistanceJoint2D ropeJoint;
    public GameObject[] ropeTrys;
    InputFrame input;
    bool isFailedPlaying;

    public AudioSource ropeSound;

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
        input = player.input;
        if (playerMovement.isGround && !playerMovement.isSwinging)
        {
            ropeStateManager.ResetRopeTries();
            ropeStateManager.UpdateRopeTries();
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

        // Überprüfen und entfernen Sie ungültige Seilpunkte
        CheckAndRemoveInvalidRopePoints();

        ropePointManager.UpdateRopePositions();

        if (ropePointManager.GetRopePointCount() > 1)
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

        var x = transform.position.x + 1f * Mathf.Cos(aimAngle);
        var y = transform.position.y + 1f * Mathf.Sin(aimAngle);

        var crossHairPosition = new Vector3(x, y, 0);
        crosshair.transform.position = crossHairPosition;
    }

    public void HandleSwinging()
    {
        CheckAndRemoveInvalidRopePoints();
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
            }
            if (playerToCurrentNextHit)
            {
                var enemy = playerToCurrentNextHit.collider.GetComponent<RangedEnemyController>();
                if (enemy != null)
                {
                    enemyInteractionHandler.HandleEnemyHit(enemy.gameObject);
                    PlayHitAnimation();
                    ropeStateManager.ResetRope();
                }

                var colliderWithVertices = playerToCurrentNextHit.collider as PolygonCollider2D;
                if (colliderWithVertices != null)
                {
                    var closestPointToHit = GetClosestColliderPointFromRaycastHit(playerToCurrentNextHit, colliderWithVertices);

                    if (ropePointManager.ContainsPoint(closestPointToHit))
                    {
                        ropeStateManager.ResetRope();
                    }

                    ropePointManager.AddRopePoint(closestPointToHit.position, playerToCurrentNextHit.normal, playerToCurrentNextHit.collider.transform);

                }
            }
        }
    }
   
    private void PlayHitAnimation()
    {
        // Platzhalter Methode für die Animation
    }

    private void UnwrapRopeSegment()
    {
        var lastWrapPoint = ropePointManager.ropePoints.Last();
        var distanceToLastPoint = Vector2.Distance(playerPosition, lastWrapPoint.position);

        float unwrapDistanceThreshold = 0.5f;

        if (distanceToLastPoint < unwrapDistanceThreshold)
        {
            Destroy(lastWrapPoint.gameObject);
            ropePointManager.ropePoints.RemoveAt(ropePointManager.ropePoints.Count - 1);
            ropePointManager.wrapPointsLookup.Remove(lastWrapPoint);
            ropePointManager.distanceSet = false;
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
        if (ropeStateManager.IsRopeAttached())
        {
            ropeJoint.distance -= climbSpeed * Time.deltaTime;
        }
    }

    public void ResetRope()
    {
        ropeJoint.enabled = false;
        playerMovement.isSwinging = false;
        ropeRenderer.positionCount = 2;
        ropeRenderer.SetPosition(0, transform.position);
        ropeRenderer.SetPosition(1, transform.position);
        ropeStateManager.ResetRope();
        ropePointManager.ClearRopePoints();
    }
    private void CheckAndRemoveInvalidRopePoints()
    {
        if (ropePointManager.RemoveInvalidRopePoints())
        {
            ResetRope();
        }
    }
}

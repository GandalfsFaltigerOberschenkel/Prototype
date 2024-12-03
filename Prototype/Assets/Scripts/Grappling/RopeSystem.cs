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
    private Vector2 playerPosition;
    private Rigidbody2D ropeHingeAnchorRb;
    private SpriteRenderer ropeHingeAnchorSprite;
    public LineRenderer ropeRenderer;
    public LayerMask ropeLayerMask;
    public LayerMask coinLayerMask; // Neue LayerMask für Coin
    private float ropeMaxCastDistance = 5f;
    private List<Transform> ropePoints = new List<Transform>();
    private bool distanceSet;
    private Dictionary<Transform, int> wrapPointsLookup = new Dictionary<Transform, int>();
    public float climbSpeed = 3f;
    public float pushForce = 10f;
    public PlayerController player;
    public LayerMask destroyRopeMask;
    public GameObject[] ropeTrys;
    private int remainingRopeTries;
    private EnemyController currentEnemy;
    public int damage = 50;
    public RopeStateManager ropeStateManager;
    public RopePointManager ropePointManager;
    private RopeCollisionHandler ropeCollisionHandler;
    private EnemyInteractionHandler enemyInteractionHandler;
    private RopeCoinCollector ropeCoinCollector;

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
        remainingRopeTries = ropeTrys.Length;

        ropeStateManager.Initialize(ropeTrys, playerMovement, ropeRenderer, ropeJoint, ropeHingeAnchorSprite, ropePointManager);
        ropePointManager.Initialize(ropeHingeAnchorRb, ropeRenderer, ropeJoint, ropeHingeAnchorSprite, transform);

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

        if (!ropeStateManager.IsRopeAttached())
        {
            SetCrosshairPosition(aimAngle);
            playerMovement.isSwinging = false;
        }
        else
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

    public void HandleSwingButtonHeld(Vector2 aimDirection)
    {
        if (ropeStateManager.IsRopeAttached() || ropeStateManager.GetRemainingRopeTries() <= 0) return;
        ropeRenderer.enabled = true;

        var hit = Physics2D.Raycast(playerPosition, aimDirection, ropeMaxCastDistance, ropeLayerMask);
        var checkDestroyRopeObj = Physics2D.Raycast(playerPosition, aimDirection, ropeMaxCastDistance, destroyRopeMask);

        if (checkDestroyRopeObj)
        {
            ropeStateManager.ResetRope();
            return;
        }

        if (hit.collider != null)
        {
            var coinHits = Physics2D.RaycastAll(playerPosition, aimDirection, ropeMaxCastDistance, coinLayerMask);

            foreach (var coinHit in coinHits)
            {
                if (coinHit.collider != null)
                {
                    ropeCoinCollector.CollectCoin(coinHit.collider.gameObject);
                    ResetRope();
                }
            }

            var enemy = hit.collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemyInteractionHandler.HandleEnemyHit(enemy.gameObject);
                currentEnemy = enemy;
                ropeStateManager.AttachRope();
                ropePointManager.AddRopePoint(enemy.transform.position, enemy.transform);
                ropeJoint.distance = Vector2.Distance(playerPosition, enemy.transform.position);
                ropeJoint.enabled = true;
                ropeHingeAnchorSprite.enabled = true;
                remainingRopeTries--;
                UpdateRopeTrys();
                return;
            }

            ropeStateManager.AttachRope();
            if (!ropePointManager.ContainsPoint(hit.point))
            {
                transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, pushForce), ForceMode2D.Impulse);
                ropePointManager.AddRopePoint(hit.point, hit.collider.transform);
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
            ropeStateManager.ResetRope();
        }
    }

    private void HandleEnemyDestroyed(EnemyController enemy)
    {
        if (currentEnemy == enemy)
        {
            ResetRope();
        }
    }

    public void ResetRope()
    {
        ropeStateManager.ResetRope();
        ropePointManager.ClearRopePoints();
    }

    private void UpdateRopePositions()
    {
        if (!ropeStateManager.IsRopeAttached()) return;

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

        // Aktualisiere die Position des LineRenderer, wenn ein Feind getroffen wurde
        if (currentEnemy != null)
        {
            ropeRenderer.SetPosition(ropeRenderer.positionCount - 1, currentEnemy.transform.position);
        }
    }

    private void UnwrapRopeSegment()
    {
        var lastWrapPoint = ropePoints.Last();
        var distanceToLastPoint = Vector2.Distance(playerPosition, lastWrapPoint.position);


       
            Destroy(lastWrapPoint.gameObject);
            ropePoints.RemoveAt(ropePoints.Count - 1);
            wrapPointsLookup.Remove(lastWrapPoint);
        
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

    private void UpdateRopeTrys()
    {
        for (int i = 0; i < ropeTrys.Length; i++)
        {
            ropeTrys[i].SetActive(i < remainingRopeTries);
        }
    }
    private void CollectCoin(GameObject coin)
    {
        // Logik zum Einsammeln des Coins
        GetComponent<Purse>().AddCurrency(coin.GetComponent<Collider2D>());
        ResetRope();
    }
  
}

using UnityEngine;

public class RopeStateManager : MonoBehaviour
{
    private bool ropeAttached;
    public int remainingRopeTries;
    public GameObject[] ropeTries; // Korrigierter Name
    public GameObject extraRopeTry;
    private PlayerMovement playerMovement;
    private LineRenderer ropeRenderer;
    private DistanceJoint2D ropeJoint;
    private SpriteRenderer ropeHingeAnchorSprite;
    private RopePointManager ropePointManager;
    public RopeSystem ropeSystem;
    public LayerMask coinLayer;

    public void Initialize(GameObject[] ropeTries, PlayerMovement playerMovement, LineRenderer ropeRenderer, DistanceJoint2D ropeJoint, SpriteRenderer ropeHingeAnchorSprite, RopePointManager ropePointManager)
    {
        if (ropeTries == null || playerMovement == null || ropeRenderer == null || ropeJoint == null || ropeHingeAnchorSprite == null || ropePointManager == null)
        {
            Debug.LogError("Initialization parameters cannot be null");
            return;
        }

        this.ropeTries = ropeTries;
        this.playerMovement = playerMovement;
        this.ropeRenderer = ropeRenderer;
        this.ropeJoint = ropeJoint;
        this.ropeHingeAnchorSprite = ropeHingeAnchorSprite;
        this.ropePointManager = ropePointManager;
        remainingRopeTries = ropeTries.Length;
        UpdateRopeTries();
    }

    public void ResetRope()
    {
        Debug.Log("Rope reset");
        ropeJoint.enabled = false;
        ropeAttached = false;
        playerMovement.isSwinging = false;
        ropeRenderer.positionCount = 2;
        ropeRenderer.SetPosition(0, transform.position);
        ropeRenderer.SetPosition(1, transform.position);
        ropePointManager.ClearRopePoints();
        ropeHingeAnchorSprite.enabled = false;
    }

    public void ResetRopeTries()
    {
        remainingRopeTries = ropeTries.Length;
        UpdateRopeTries();
    }

    public void AttachRope()
    {
        Debug.Log("Rope attached");
        ropeAttached = true;
        remainingRopeTries--;
        UpdateRopeTries();
    }

    public bool IsRopeAttached()
    {
        return ropeAttached;
    }

    public int GetRemainingRopeTries()
    {
        return remainingRopeTries;
    }

    public void UpdateRopeTries()
    {
        for (int i = 0; i < ropeTries.Length; i++)
        {
            ropeTries[i].SetActive(i < remainingRopeTries);
        }
    }

    public void HandleSwingButtonHeld(Vector2 aimDirection)
    {
        if (IsRopeAttached() || remainingRopeTries <= 0) return;
        ropeRenderer.enabled = true;

        var hit = Physics2D.Raycast(playerMovement.transform.position, aimDirection, ropeSystem.ropeMaxCastDistance, ropeSystem.ropeLayerMask);
        var checkDestroyRopeObj = Physics2D.Raycast(playerMovement.transform.position, aimDirection, ropeSystem.ropeMaxCastDistance, ropeSystem.destroyRopeMask);
        var checkEnemy = Physics2D.Raycast(playerMovement.transform.position, aimDirection, ropeSystem.ropeMaxCastDistance, ropeSystem.ropeLayerMask);
        var checkCoin = Physics2D.Raycast(playerMovement.transform.position, aimDirection, ropeSystem.ropeMaxCastDistance, coinLayer);
        if (checkEnemy.collider.GetComponent<RangedEnemyController>() != null)
        {
            checkEnemy.collider.GetComponent<RangedEnemyController>().TakeDamage(ropeSystem.damage);
        }
        if(checkCoin.collider != null)
        {
            ropeSystem.ropeCoinCollector.CollectCoin(checkCoin.collider.gameObject);
        }
        if (hit.collider != null && !checkDestroyRopeObj)
        {
            ropePointManager.AddRopePoint(hit.point,hit.normal, hit.collider.transform);
            ropeJoint.enabled = true;
            ropeJoint.connectedBody = ropeSystem.ropeHingeAnchorRb;
            ropeHingeAnchorSprite.enabled = true;
            ropeSystem.ropeHingeAnchorRb.transform.position = hit.point;
            ropeJoint.distance = Vector2.Distance(playerMovement.transform.position, hit.point);
            AttachRope();
        }
    }
}

using UnityEngine;

public class RopeStateManager : MonoBehaviour
{
    private bool ropeAttached;
    private int remainingRopeTries;
    private GameObject[] ropeTrys;
    private PlayerMovement playerMovement;
    private LineRenderer ropeRenderer;
    private DistanceJoint2D ropeJoint;
    private SpriteRenderer ropeHingeAnchorSprite;
    private RopePointManager ropePointManager;

    public void Initialize(GameObject[] ropeTrys, PlayerMovement playerMovement, LineRenderer ropeRenderer, DistanceJoint2D ropeJoint, SpriteRenderer ropeHingeAnchorSprite, RopePointManager ropePointManager)
    {
        this.ropeTrys = ropeTrys;
        this.playerMovement = playerMovement;
        this.ropeRenderer = ropeRenderer;
        this.ropeJoint = ropeJoint;
        this.ropeHingeAnchorSprite = ropeHingeAnchorSprite;
        this.ropePointManager = ropePointManager;
        remainingRopeTries = ropeTrys.Length;
        UpdateRopeTrys();
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
        remainingRopeTries = ropeTrys.Length;
        UpdateRopeTrys();
    }

    public void AttachRope()
    {
        Debug.Log("Rope attached");
        ropeAttached = true;
        remainingRopeTries--;
        UpdateRopeTrys();
    }

    public bool IsRopeAttached()
    {
        return ropeAttached;
    }

    public int GetRemainingRopeTries()
    {
        return remainingRopeTries;
    }

    private void UpdateRopeTrys()
    {
        for (int i = 0; i < ropeTrys.Length; i++)
        {
            ropeTrys[i].SetActive(i < remainingRopeTries);
        }
    }
}

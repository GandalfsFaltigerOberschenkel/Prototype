using UnityEngine;

public class JumpUpgrade : UpgradeBase
{
    public override void ApplyUpgrade()
    {
        playerController.playerMovement.jumpForce *= multiplier;
        base.ApplyUpgrade();
    }
    public override void DeapplyUpgrade()
    {
        playerController.playerMovement.jumpForce /= multiplier;
        base.DeapplyUpgrade();
    }
}

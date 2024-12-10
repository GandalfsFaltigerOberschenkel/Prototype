using UnityEngine;

public class SpeedUpgrade : UpgradeBase
{
    public override void ApplyUpgrade()
    {
        playerController.playerMovement.maxHorizontalSpeed *= multiplier;
        playerController.GetComponent<RopeSystem>().climbSpeed *= multiplier;
        base.ApplyUpgrade();
    }
    public override void DeapplyUpgrade()
    {
        playerController.playerMovement.maxHorizontalSpeed /= multiplier;
        playerController.GetComponent<RopeSystem>().climbSpeed /= multiplier;
        base.DeapplyUpgrade();
    }

}

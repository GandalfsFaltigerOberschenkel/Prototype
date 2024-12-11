using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class ExtraTryUpgrade : UpgradeBase
{
    public int extraTries = 1;

    public override void ApplyUpgrade()
    {
        base.ApplyUpgrade();
        RopeStateManager ropeStateManager = FindObjectOfType<RopeStateManager>();
        if (ropeStateManager != null)
        {
            List<GameObject> obj = ropeStateManager.ropeTries.ToList();
            ropeStateManager.extraRopeTry.SetActive(true);
            obj.Add(ropeStateManager.extraRopeTry);
            ropeStateManager.ropeTries = obj.ToArray();

            ropeStateManager.UpdateRopeTries();
        }
    }
    public override void DeapplyUpgrade()
    {
        base.DeapplyUpgrade();
        RopeStateManager ropeStateManager = FindObjectOfType<RopeStateManager>();
        if (ropeStateManager != null)
        {
            List<GameObject> obj = ropeStateManager.ropeTries.ToList();
            ropeStateManager.extraRopeTry.SetActive(false);
            obj.Remove(ropeStateManager.extraRopeTry);
            ropeStateManager.ropeTries = obj.ToArray();
            
            ropeStateManager.UpdateRopeTries();
        }
        base.DeapplyUpgrade();
    }
}


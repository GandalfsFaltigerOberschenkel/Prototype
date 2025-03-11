using UnityEngine;

public class CreditsPanel : UIPanel
{
   
    public override void KillPanel()
    {
        if (UIManager.instance.isMenu)
        {
            UIManager.instance.CloseAllPanels();
        }
        else
        {
            base.KillPanel();
        }
        
    }
}

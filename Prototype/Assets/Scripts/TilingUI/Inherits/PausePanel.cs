using UnityEngine;

public class PausePanel : UIPanel
{
    // �ffne das Einstellungen Panel
    public void OpenSettings()
    {
        UIManager.instance.OpenPanel(1); // Panel ID 1 f�r Einstellungen
    }
    public override void KillPanel()
    {

        GameManager2.Instance.TogglePauseGame();
        
    }
}

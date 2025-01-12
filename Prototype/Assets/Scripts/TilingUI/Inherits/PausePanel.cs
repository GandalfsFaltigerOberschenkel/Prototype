using UnityEngine;

public class PausePanel : UIPanel
{
    // Öffne das Einstellungen Panel
    public void OpenSettings()
    {
        UIManager.instance.OpenPanel(1); // Panel ID 1 für Einstellungen
    }
    public override void KillPanel()
    {

        GameManager2.Instance.TogglePauseGame();
        
    }
}

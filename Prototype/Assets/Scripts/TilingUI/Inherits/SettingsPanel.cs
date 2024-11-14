using UnityEngine;

public class SettingsPanel : UIPanel
{
    // Öffne das Grafik-Einstellungen Panel
    public void OpenGraphicSettings()
    {
        UIManager.instance.OpenPanel(2); // Panel ID 2 für Grafik-Einstellungen
    }

    // Öffne das Audio-Einstellungen Panel
    public void OpenAudioSettings()
    {
        UIManager.instance.OpenPanel(3); // Panel ID 3 für Audio-Einstellungen
    }
}

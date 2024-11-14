using UnityEngine;

public class SettingsPanel : UIPanel
{
    // �ffne das Grafik-Einstellungen Panel
    public void OpenGraphicSettings()
    {
        UIManager.instance.OpenPanel(2); // Panel ID 2 f�r Grafik-Einstellungen
    }

    // �ffne das Audio-Einstellungen Panel
    public void OpenAudioSettings()
    {
        UIManager.instance.OpenPanel(3); // Panel ID 3 f�r Audio-Einstellungen
    }
}

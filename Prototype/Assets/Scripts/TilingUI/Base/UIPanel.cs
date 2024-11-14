using UnityEngine;
using TMPro;

public class UIPanel : MonoBehaviour
{
    public int id; // Panel ID
    public string title; // Panel Titel
    TMP_Text titleText; // Textkomponente für den Titel
    public GameObject content; // Inhalt des Panels

    public void Start()
    {
        InitializeTitle();
    }

    private void InitializeTitle()
    {
        titleText = GetComponentInChildren<TMP_Text>(); // Finde die Textkomponente im Panel
        titleText.text = title; // Setze den Titeltext
    }

    // Schließe das Panel
    public void KillPanel()
    {
        UIManager.instance.RemovePanel(id); // Entferne das Panel aus der Liste der aktiven Panels
        Destroy(this.gameObject); // Zerstöre das Panel GameObject
    }
}

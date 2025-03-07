using UnityEngine;
using TMPro;

public class UIPanel : MonoBehaviour
{
    public int id; // Panel ID
    public string title; // Panel Titel
    TMP_Text titleText; // Textkomponente f�r den Titel
    public GameObject content; // Inhalt des Panels
    public bool isFocused = false;

    public virtual void Start()
    {
        InitializeTitle();
    }

    private void InitializeTitle()
    {
        titleText = GetComponentInChildren<TMP_Text>(); // Finde die Textkomponente im Panel
        titleText.text = title; // Setze den Titeltext
    }

    // Schlie�e das Panel
    public virtual void KillPanel()
    {
        UIManager.instance.ClosePanel(id);
        UIManager.instance.RemovePanel(id); // Entferne das Panel aus der Liste der aktiven Panels
        
        Destroy(this.gameObject); // Zerst�re das Panel GameObject
    }
}

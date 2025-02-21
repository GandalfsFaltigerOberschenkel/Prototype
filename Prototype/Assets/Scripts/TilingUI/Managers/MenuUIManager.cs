using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.UI;
using JetBrains.Annotations;
using UnityEngine.EventSystems;
public class MenuUIManager : MonoBehaviour
{
    #region Singleton
    public static MenuUIManager instance;

    private void Awake()
    {
        InitializeSingleton();
    }

    private void InitializeSingleton()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    public List<UIPanel> activePanels = new List<UIPanel>();
    public GameObject[] availablePanels = new GameObject[0];
    public Transform panelSpace;

    public int maxPanels = 3; // Maximale Anzahl an Panels, die gleichzeitig geöffnet sein dürfen

    private void Update()
    {
        //Beispiel: Pause Menü bei Escape
        //HandleEscapeKey();
    }

    private void HandleEscapeKey()
    {
        //Beispiel: Pause Menü bei Escape
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            TogglePausePanel();
        }
    }
    //Beispiel: Wie Rufe ich Panels von einer Taste auf
    public void TogglePausePanel()
    {
        //Finde Pause Panel
        UIPanel panel = activePanels.FirstOrDefault(e => e.id == 0);
        bool isPauseAlreadyActive = panel == null; //Ist ein Pause Panel offen?
        if (isPauseAlreadyActive)
        {
            OpenPanel(0); //Öffne Pause Panel
        }
        else
        {

            CloseAllPanels();
        }

    }
    //Öffne Panel mit der ID
    public void OpenPanel(int id)
    {

        if (IsPanelAlreadyOpen(id)) //Ist der Panel bereits geöffnet?
        {
            Debug.LogWarning("Panel mit derselben ID ist bereits geöffnet.");
            return; //Beende die Funktion
        }

        //Tausche das letzte Panel in der Liste aus, wenn die Maximale Anzahl an Panels erreicht ist
        if (IsMaxPanelsReached())
        {
            CloseLastOpenedPanel(); //Schließe das letzte Panel in der Liste
        }

        InstantiateAndAddPanel(id); //Erschaffe und füge das Panel der Liste hinzu
    }
    //Check ob der Panel bereits geöffnet ist
    private bool IsPanelAlreadyOpen(int id)
    {
        return activePanels.Any(panel => panel.id == id);
    }
    //Check ob die Maximale Anzahl an aktiven Panels erreicht ist
    private bool IsMaxPanelsReached()
    {
        return activePanels.Count >= maxPanels;
    }
    //Schließe den letzten Panel in der Liste
    private void CloseLastOpenedPanel()
    {
        UIPanel lastPanel = activePanels.Last();
        ClosePanel(lastPanel.id);
    }

    private void InstantiateAndAddPanel(int id)
    {
        //Finde den Richtigen Panel mit der ID
        GameObject availablePanel = availablePanels.FirstOrDefault(p => p.GetComponent<UIPanel>().id == id);
        if (availablePanel != null) //Existiert so ein Panel?
        {
            GameObject newPanel = Instantiate(availablePanel, panelSpace); //Erschaffe das Panel Prefab
            newPanel.GetComponent<UIPanel>().isFocused = true;
            foreach (UIPanel panel in activePanels)
            {
                panel.isFocused = false;
            }
            GameObject[] buttons = newPanel.GetComponentsInChildren<Button>().Select(button => button.gameObject).ToArray();
            EventSystem eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(buttons[0]);

            activePanels.Add(newPanel.GetComponent<UIPanel>()); //Füge das Panel der aktiven Panel Liste hinzu

        }
    }

    //Schließe Panel mit der ID
    public void ClosePanel(int id)
    {
        // Find the panel with the given ID
        UIPanel panel = activePanels.FirstOrDefault(e => e.id == id);
        if (panel != null) // Panel found in the list?
        {

            Destroy(panel.gameObject); // Destroy the panel GameObject (this will call RemovePanel method as well
            // Remove the panel from the activePanels list
            activePanels.Remove(panel);

            // Set the selected GameObject to the last panel in the list if there are any panels left
            if (activePanels.Count > 0)
            {
                UIPanel lastPanel = activePanels.Last();
                lastPanel.isFocused = true;
                GameObject[] buttons = lastPanel.GetComponentsInChildren<Button>().Select(button => button.gameObject).ToArray();
                EventSystem.current.SetSelectedGameObject(buttons[0]);
            }
            panel.isFocused = false;

        }
    }

    //Entferne Panel aus der Liste, wenn es geschlossen wurde
    public void RemovePanel(int id)
    {
        activePanels.RemoveAll(e => e.id == id);
    }

    //Schließe alle Panels in der Liste
    private void CloseAllPanels()
    {
        foreach (var panel in activePanels.ToList())
        {
            UIManager.instance.RemovePanel(panel.id); // Entferne das Panel aus der Liste der aktiven Panels
            Destroy(panel.gameObject); // Zerstöre das Panel GameObject
        }
        activePanels.Clear();
    }
}

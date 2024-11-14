using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class UIManager : MonoBehaviour
{
    #region Singleton
    public static UIManager instance;

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
    public int maxPanels = 3; // Maximale Anzahl an Panels, die gleichzeitig geˆffnet sein d¸rfen

    private void Update()
    {
        //Beispiel: Pause Men¸ bei Escape
        HandleEscapeKey();
    }

    private void HandleEscapeKey()
    {
        //Beispiel: Pause Men¸ bei Escape
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            TogglePausePanel();
        }
    }
    //Beispiel: Wie Rufe ich Panels von einer Taste auf
    private void TogglePausePanel()
    {
        //Finde Pause Panel
        UIPanel panel = activePanels.FirstOrDefault(e => e.id == 0);
        bool isPauseAlreadyActive = panel == null; //Ist ein Pause Panel offen?
        if (isPauseAlreadyActive)
        {
            OpenPanel(0); //÷ffne Pause Panel
        }
        else
        {
            panel.KillPanel(); //Schlieﬂe Pause Panel
        }
    }
    //÷ffne Panel mit der ID
    public void OpenPanel(int id)
    {
        
        if (IsPanelAlreadyOpen(id)) //Ist der Panel bereits geˆffnet?
        {
            Debug.LogWarning("Panel mit derselben ID ist bereits geˆffnet.");
            return; //Beende die Funktion
        }

        //Tausche das letzte Panel in der Liste aus, wenn die Maximale Anzahl an Panels erreicht ist
        if (IsMaxPanelsReached())
        {
            CloseLastOpenedPanel(); //Schlieﬂe das letzte Panel in der Liste
        }

        InstantiateAndAddPanel(id); //Erschaffe und f¸ge das Panel der Liste hinzu
    }
    //Check ob der Panel bereits geˆffnet ist
    private bool IsPanelAlreadyOpen(int id)
    {
        return activePanels.Any(panel => panel.id == id);
    }
    //Check ob die Maximale Anzahl an aktiven Panels erreicht ist
    private bool IsMaxPanelsReached()
    {
        return activePanels.Count >= maxPanels;
    }
    //Schlieﬂe den letzten Panel in der Liste
    private void CloseLastOpenedPanel()
    {
        UIPanel lastPanel = activePanels.Last();
        lastPanel.KillPanel();
    }

    private void InstantiateAndAddPanel(int id)
    {
        //Finde den Richtigen Panel mit der ID
        GameObject availablePanel = availablePanels.FirstOrDefault(p => p.GetComponent<UIPanel>().id == id);
        if (availablePanel != null) //Existiert so ein Panel?
        {
            GameObject newPanel = Instantiate(availablePanel, panelSpace); //Erschaffe das Panel Prefab
            activePanels.Add(newPanel.GetComponent<UIPanel>()); //F¸ge das Panel der aktiven Panel Liste hinzu
        }
    }

    //Schlieﬂe Panel mit der ID
    public void ClosePanel(int id)
    {
        //Finde Panel mit der ID
        UIPanel panel = activePanels.FirstOrDefault(e => e.id == id);
        if (panel != null) //Panel in der Liste gefunden?
        {
            panel.KillPanel(); //Schlieﬂe das Panel
        }
    }

    //Entferne Panel aus der Liste, wenn es geschlossen wurde
    public void RemovePanel(int id)
    {
        activePanels.RemoveAll(e => e.id == id);
    }

    //Schlieﬂe alle Panels in der Liste
    private void CloseAllPanels()
    {
        foreach (var panel in activePanels.ToList())
        {
            panel.KillPanel();
        }
        activePanels.Clear();
    }
}

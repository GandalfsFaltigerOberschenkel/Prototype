using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.UI;
using JetBrains.Annotations;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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
    public bool isMenu = false;
    public GameObject timer;
    public GameObject secretLeaderboardButton;
    public AudioSource blibSound;

    public int maxPanels = 3; // Maximale Anzahl an Panels, die gleichzeitig ge�ffnet sein d�rfen

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if(PlayerPrefs.GetInt("isAble", 0) == 1)
            {
                PlayerPrefs.SetInt("isAble", 0);
                secretLeaderboardButton.SetActive(false);
            }
            else
            {
                PlayerPrefs.SetInt("isAble", 1);
                secretLeaderboardButton.SetActive(true);
            }
         
        }
    }
    private void Start()
    {
        bool isAble = PlayerPrefs.GetInt("isAble", 0) == 1;
        if (isAble)
        {
            secretLeaderboardButton.SetActive(true);
        }
        FindAllButtonsAndSetEvents();
    }
    public void FindAllButtonsAndSetEvents()
    {
        GameObject[] buttons = Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Select(button => button.gameObject).ToArray();
        foreach (GameObject button in buttons)
        {
            Button b = button.GetComponent<Button>();
            if (b != null)
            {
                b.onClick.AddListener(() => blibSound.Play());
                EventTrigger trigger = button.AddComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerEnter
                };
                entry.callback.AddListener((eventData) => blibSound.Play());
                trigger.triggers.Add(entry);
            }
        }
    }
    public void LoadLeaderBoard()
    {
        SceneManager.LoadScene("LeaderboardSceneReadOnly");

    }
    private void HandleEscapeKey()
    {
        //Beispiel: Pause Men� bei Escape
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
            
            OpenPanel(0); //�ffne Pause Panel
        }
        else
        {
         
            CloseAllPanels();
        }
       
    }
    //�ffne Panel mit der ID
    public void OpenPanel(int id)
    {
        
           panelSpace.GetComponent<Image>().enabled = true;
        
        if (IsPanelAlreadyOpen(id)) //Ist der Panel bereits ge�ffnet?
        {
            Debug.LogWarning("Panel mit derselben ID ist bereits ge�ffnet.");
            return; //Beende die Funktion
        }

        //Tausche das letzte Panel in der Liste aus, wenn die Maximale Anzahl an Panels erreicht ist
        if (IsMaxPanelsReached())
        {
            CloseLastOpenedPanel(); //Schlie�e das letzte Panel in der Liste
        }

        InstantiateAndAddPanel(id); //Erschaffe und f�ge das Panel der Liste hinzu
    }
    //Check ob der Panel bereits ge�ffnet ist
    private bool IsPanelAlreadyOpen(int id)
    {
        return activePanels.Any(panel => panel.id == id);
    }
    //Check ob die Maximale Anzahl an aktiven Panels erreicht ist
    private bool IsMaxPanelsReached()
    {
        return activePanels.Count >= maxPanels;
    }
    //Schlie�e den letzten Panel in der Liste
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
            foreach(UIPanel panel in activePanels)
            {
                panel.isFocused = false;
            }
            GameObject[] buttons =  newPanel.GetComponentsInChildren<Button>().Select(button => button.gameObject).ToArray();
            EventSystem eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(buttons[0]);
            
            activePanels.Add(newPanel.GetComponent<UIPanel>()); //F�ge das Panel der aktiven Panel Liste hinzu
            
        }
        FindAllButtonsAndSetEvents();
    }

    //Schlie�e Panel mit der ID
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
        FindAllButtonsAndSetEvents();
    }
    
    //Entferne Panel aus der Liste, wenn es geschlossen wurde
    public void RemovePanel(int id)
    {
        activePanels.RemoveAll(e => e.id == id);
    }

    //Schlie�e alle Panels in der Liste
    public void CloseAllPanels()
    {
      
            panelSpace.GetComponent<Image>().enabled = false;
        
        foreach (var panel in activePanels.ToList())
        {
            UIManager.instance.RemovePanel(panel.id); // Entferne das Panel aus der Liste der aktiven Panels
            Destroy(panel.gameObject); // Zerst�re das Panel GameObject
        }
        activePanels.Clear();
    }
}

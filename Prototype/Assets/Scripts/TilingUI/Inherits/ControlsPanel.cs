using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SaveableSettings
{
    public bool isMultiplayer = false;
    public bool player1Keyboard = true;
    public bool player2Keyboard = false;
}

public class ControlsPanel : UIPanel
{
    public SaveableSettings currentUnsaved = new SaveableSettings();
    public Button singlePlayerButton;
    public Button multiPlayerButton;
    [SerializeField] private Button player1KeyboardBtn;
    [SerializeField] private Button player1ControllerBtn;
    [SerializeField] private Button player2KeyboardBtn;
    [SerializeField] private Button player2ControllerBtn;
    [SerializeField] private GameObject player2Panel;

    private void Start()
    {
        LoadSettings();
        UpdateChoosable();
    }

    public void SetMultiplayer(bool yes)
    {
        currentUnsaved.isMultiplayer = yes;
        if (!yes)
        {
            currentUnsaved.player2Keyboard = false; // Reset P2 when switching to Singleplayer
        }
        UpdateChoosable();
    }

    public void SetPlayer1Keyboard(bool yes)
    {
        currentUnsaved.player1Keyboard = yes;
        currentUnsaved.player2Keyboard = !yes; // Ensure P2 gets the opposite
        UpdateChoosable();
    }

    public void SetPlayer2Keyboard(bool yes)
    {
        if (!currentUnsaved.isMultiplayer) return; // P2 shouldn't be modified in Singleplayer mode

        currentUnsaved.player2Keyboard = yes;
        currentUnsaved.player1Keyboard = !yes; // Ensure P1 gets the opposite
        UpdateChoosable();
    }

    void UpdateChoosable()
    {
        Debug.Log($"Multiplayer: {currentUnsaved.isMultiplayer}, P1 Keyboard: {currentUnsaved.player1Keyboard}, P2 Keyboard: {currentUnsaved.player2Keyboard}");

        // Enable/Disable Multiplayer Panel
        player2Panel.SetActive(currentUnsaved.isMultiplayer);

        if (!currentUnsaved.isMultiplayer)
        {
            singlePlayerButton.interactable = false;
            multiPlayerButton.interactable = true;
            // Singleplayer: P1 can choose freely, P2 is disabled
            if (currentUnsaved.player1Keyboard)
            {
                player1KeyboardBtn.interactable = false;
                player1ControllerBtn.interactable = true;
            }
            else
            {
                player1KeyboardBtn.interactable = true;
                player1ControllerBtn.interactable = false;
            }



            player2KeyboardBtn.interactable = false;
            player2ControllerBtn.interactable = false;
        }
        else
        {
            singlePlayerButton.interactable = true;

            // Multiplayer: Enforce rules
            bool p1IsKeyboard = currentUnsaved.player1Keyboard;
            bool p2IsKeyboard = currentUnsaved.player2Keyboard;

            // Player 1 Selection
            player1KeyboardBtn.interactable = !p1IsKeyboard; // Can select if not already selected
            player1ControllerBtn.interactable = p1IsKeyboard;

            // Player 2 Selection (Opposite of Player 1)
            player2KeyboardBtn.interactable = !p2IsKeyboard;
            player2ControllerBtn.interactable = p2IsKeyboard;
        }
    }
    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("SavedControlSettings"))
        {
            string json = PlayerPrefs.GetString("SavedControlSettings");
            currentUnsaved = JsonUtility.FromJson<SaveableSettings>(json);
        }
        // Else, use default values already set in currentUnsaved
    }
    public void Save()
    {
        if (GameManager2.Instance != null)
        {
            // Update GameManager based on current settings
            GameManager2.Instance.playerNum = currentUnsaved.isMultiplayer ? 2 : 1;
            GameManager2.Instance.useController = !currentUnsaved.player1Keyboard;
            GameManager2.Instance.Set();
        }
        // Save settings to PlayerPrefs
        string json = JsonUtility.ToJson(currentUnsaved);
        PlayerPrefs.SetString("SavedControlSettings", json);
        PlayerPrefs.Save(); // Ensures data is written immediately
        
        UIManager.instance.ClosePanel(this.id);


        KillPanel();
    }
}

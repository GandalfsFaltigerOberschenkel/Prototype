using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager2 : MonoBehaviour
{
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

         // Load settings before initializing
        Set();
    }
    public void Set()
    {
        players = new List<Player>();
        keyboard = new KeyboardDevice();
        controller = new ControllerDevice();
        SetupPlayers();
    }

    KeyboardDevice keyboard;
    ControllerDevice controller;

    public List<ControllerDevice> controllerDevices;

    public static GameManager2 Instance;
    public int playerNum = 2;
    public Player playerPrefab;
    public MergedCharacter mergedCharacterPrefab;
    public List<Player> players;
    public bool useController = true;
    public Transform spawnpos;
    public UIManager uiManager;
    public MergedCharacter spawnedChar;
    public CinemachineCamera cam;
    public List<GameObject> platforms;
    public Volume volume;

    public AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup sfxMixerGroup;

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("SavedControlSettings"))
        {
            string json = PlayerPrefs.GetString("SavedControlSettings");
            SaveableSettings loadedSettings = JsonUtility.FromJson<SaveableSettings>(json);
            playerNum = loadedSettings.isMultiplayer ? 2 : 1;
            useController = !loadedSettings.player1Keyboard;
        }
        else
        {
            // Apply default settings if none are saved
            playerNum = 1; // Default is single-player
            useController = false; // Default is keyboard for P1
        }
        float amount = PlayerPrefs.GetFloat("MusicVolume");
        musicMixerGroup.audioMixer.SetFloat("MusicVolume", amount);


        float amount2 = PlayerPrefs.GetFloat("SFXVolume");
        sfxMixerGroup.audioMixer.SetFloat("SFXVolume", amount2);
        
        string resolution = PlayerPrefs.GetString("Resolution", "1920x1080");
        bool isFullScreen = Convert.ToBoolean(PlayerPrefs.GetInt("FullScreen", 1));
        Debug.Log("Resolution: " + resolution);
        int[] values = resolution.Split("x").ToList().Select(e => Convert.ToInt32(e)).ToArray();
        Screen.SetResolution(values[0], values[1], isFullScreen);

    }

    public static void TurnOfAllPlatforms()
    {
        foreach (GameObject platform in Instance.platforms)
        {
            platform.GetComponent<PolygonCollider2D>().excludeLayers = LayerMask.GetMask("Player");
        }
    }
    public static void TurnOnAllPlatforms()
    {
        foreach (GameObject platform in Instance.platforms)
        {
            platform.GetComponent<PolygonCollider2D>().excludeLayers = 0;
        }
    }
    private void Start()
    {
        controllerDevices = new List<ControllerDevice>();
        LoadSettings();
    }
    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            TogglePauseGame();

        }
    }
    public void JustPauseTheGame()
    {
        if (spawnedChar.GetComponent<PlayerController>().isPaused)
        {

            spawnedChar.GetComponent<PlayerController>().isPaused = false;
            spawnedChar.GetComponent<Rigidbody2D>().simulated = true;
            FindAnyObjectByType<TimerThing>().timer.Start();

        }

        else
        {

            spawnedChar.GetComponent<PlayerController>().isPaused = true;
            spawnedChar.GetComponent<Rigidbody2D>().simulated = false;
            FindAnyObjectByType<TimerThing>().timer.Stop();

        }
    }
    public void TogglePauseGame()
    {
        if (spawnedChar.GetComponent<PlayerController>().isPaused)
        {
            uiManager.TogglePausePanel();
            spawnedChar.GetComponent<PlayerController>().isPaused = false;
            spawnedChar.GetComponent<Rigidbody2D>().simulated = true;
            FindAnyObjectByType<TimerThing>().timer.Start();
            VolumeProfile volumeProfile = volume.profile;
            ColorAdjustments a;
            Bloom b;
            DepthOfField c;
            if (!volumeProfile.TryGet(out a) || !volumeProfile.TryGet(out b) || !volumeProfile.TryGet(out c)) throw new System.NullReferenceException("Color Adjust, Depth of Field or Bloom is null");
            b.intensity.Override(0);
            a.saturation.Override(0);
            c.active = false;
            UpgradeUIController.Instance.ShowUI();
        }

        else
        {
            uiManager.TogglePausePanel();
            spawnedChar.GetComponent<PlayerController>().isPaused = true;
            spawnedChar.GetComponent<Rigidbody2D>().simulated = false;
            FindAnyObjectByType<TimerThing>().timer.Stop();
            VolumeProfile volumeProfile = volume.profile;
            ColorAdjustments a;
            Bloom b;
            DepthOfField c;
            if (!volumeProfile.TryGet(out a) || !volumeProfile.TryGet(out b) || !volumeProfile.TryGet(out c)) throw new System.NullReferenceException("Color Adjust, Depth of Field or Bloom is null");
            c.active = true;
            a.saturation.Override(-100);
            b.intensity.Override(3);
            UpgradeUIController.Instance.HideUI();
        }

    }

    public List<IInputDevice> GetControllerDevices()
    {
        string[] joystickNames = Input.GetJoystickNames();
        List<IInputDevice> controllers = new List<IInputDevice>
        {
            new KeyboardDevice()
        };

        foreach (string joystickName in joystickNames)
        {
            ControllerDevice controller = new ControllerDevice();
            controller.Name = joystickName;
            controllers.Add(controller);
        }

        return controllers;
    }

    void SetupPlayers()
    {
        if (spawnedChar == null)
        {
            spawnedChar = Instantiate<MergedCharacter>(mergedCharacterPrefab, spawnpos.position, Quaternion.identity);
        }
        List<IInputDevice> inputDevices = GetInputDevices();
        List<InputManager> inputManagers = InputManagerFactory.CreateInputManagers(spawnedChar.gameObject, inputDevices.ToArray());
        spawnedChar.InitializeInputManagers(inputManagers);
        cam.Target.TrackingTarget = spawnedChar.transform;
    }

    List<IInputDevice> GetInputDevices()
    {
        List<IInputDevice> inputDevices = new List<IInputDevice>();

        if (playerNum == 1)
        {
            inputDevices.Add(useController ? controller : keyboard);
        }
        else if (playerNum == 2)
        {
            inputDevices.Add(useController ? controller : keyboard);
            inputDevices.Add(useController ? keyboard : controller);
        }

        return inputDevices;
    }

    public void SwitchInputDevice(int playerIndex, IInputDevice newDevice)
    {
        if (playerIndex < 0 || playerIndex >= players.Count)
        {
            Debug.LogError("Ungültiger Spielerindex");
            return;
        }

        Player player = players[playerIndex];
        InputManager inputManager = player.GetComponent<InputManager>();
        if (inputManager != null)
        {
            inputManager.Initialize(newDevice);
        }
    }
}

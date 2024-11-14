using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    private void Start()
    {
        controllerDevices = new List<ControllerDevice>();
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
        MergedCharacter mergedCharacter = Instantiate<MergedCharacter>(mergedCharacterPrefab, transform.position, Quaternion.identity);
        List<IInputDevice> inputDevices = GetInputDevices();
        List<InputManager> inputManagers = InputManagerFactory.CreateInputManagers(mergedCharacter.gameObject, inputDevices.ToArray());
        mergedCharacter.InitializeInputManagers(inputManagers);
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

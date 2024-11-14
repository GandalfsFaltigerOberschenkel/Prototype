using System.Collections.Generic;
using UnityEngine;

public class InputManagerFactory
{
    public static InputManager CreateInputManager(GameObject character, IInputDevice inputDevice)
    {
        InputManager inputManager = character.AddComponent<InputManager>();
        inputManager.Initialize(inputDevice);
        return inputManager;
    }

    public static List<InputManager> CreateInputManagers(GameObject character, IInputDevice[] inputDevices)
    {
        List<InputManager> inputManagers = new List<InputManager>();
        foreach (var inputDevice in inputDevices)
        {
            if (inputDevice != null)
            {
                InputManager inputManager = CreateInputManager(character, inputDevice);
                inputManagers.Add(inputManager);
            }
        }
        return inputManagers;
    }
}

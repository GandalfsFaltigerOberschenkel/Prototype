using System.Collections.Generic;
using UnityEngine;

public class MergedCharacter : MonoBehaviour
{
    public PlayerController player;
    public InputManager[] inputManagers;

  

    public void InitializeInputManagers(List<InputManager> inputManager)
    {

        player.inputManagers = inputManager.ToArray() ;
    }
}

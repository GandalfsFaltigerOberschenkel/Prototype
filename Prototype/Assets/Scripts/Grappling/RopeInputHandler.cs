using UnityEngine;

public class RopeInputHandler : MonoBehaviour
{
    private RopeSystem ropeSystem;

    private void Awake()
    {
        ropeSystem = GetComponent<RopeSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    // RopeInputHandler.cs
    private void HandleInput()
    {
        Vector2 aimDirection = ropeSystem.player.input.aimDirection;
        if (ropeSystem.player.input.swingButtonPressed)
        {
            ropeSystem.ropeSound.Play();
        }
        if (ropeSystem.player.input.swingButtonHeld)
        {
            Debug.Log("Swing button held");
            ropeSystem.ropeStateManager.HandleSwingButtonHeld(aimDirection);
            // Sound abspielen, wenn die Schwingtaste gedrückt wird
        }

        if (ropeSystem.player.input.swingButtonReleased)
        {
            Debug.Log("Swing button released");
            ropeSystem.ropeStateManager.ResetRope();
        }
    }
}

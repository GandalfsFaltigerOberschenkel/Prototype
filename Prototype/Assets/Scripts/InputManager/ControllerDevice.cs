using UnityEngine;

public class ControllerDevice : IInputDevice
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string Name { get; set; } = "Controller";
    public InputFrame ProcessInput()
    {
        InputFrame frame = new InputFrame();
        frame.actionButtonPressed = false;
        frame.actionButtonHeld = false;
        frame.inputDirection = Vector2.zero;
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 inputDirection = new Vector2(moveX, moveY).normalized;
        if (Input.GetButtonDown("Jump"))
        {
            frame.actionButtonPressed = true;
        }
        if (Input.GetButton("Jump"))
        {
            frame.actionButtonHeld = true;
        }
        frame.inputDirection = inputDirection;
        return frame;
    }
}

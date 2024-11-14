using UnityEngine;

public class KeyboardDevice : IInputDevice
{
    public string Name { get; set; } = "Keyboard";
    public InputFrame ProcessInput()
    {
        InputFrame frame = new InputFrame();
        frame.actionButtonPressed = false;
        frame.actionButtonHeld = false;
        frame.inputDirection = Vector2.zero;
        float moveX = Input.GetAxisRaw("HorizontalKeyboard");
        float moveY = Input.GetAxisRaw("VerticalKeyboard");
        Vector2 inputDirection = new Vector2(moveX, moveY).normalized;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            frame.actionButtonPressed = true;
        }
        if(Input.GetKey(KeyCode.Space))
        {
            frame.actionButtonHeld = true;
        }
        frame.inputDirection = inputDirection;
        return frame;
    }
}

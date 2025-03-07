using UnityEngine;

public class ControllerDevice : IInputDevice
{
    private Vector2 aimDir;
    public string Name { get; set; } = "Controller";
    public InputFrame ProcessInput()
    {
        InputFrame frame = new InputFrame();
        frame.actionButtonPressed = false;
        frame.actionButtonHeld = false;
        frame.inputDirection = Vector2.zero;

        // Bewegungseingaben
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 inputDirection = new Vector2(moveX, moveY).normalized;
        frame.inputDirection = inputDirection;

        // Sprung-Eingaben
        if (Input.GetButtonDown("Fire1"))
        {
            frame.actionButtonPressed = true;
        }
        if (Input.GetButton("Fire1"))
        {
            frame.actionButtonHeld = true;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            frame.actionButtonReleased = true;
        }

        // Zielrichtung von rechtem Stick
        float aimX = Input.GetAxisRaw("HorizontalAim");
        float aimY = Input.GetAxisRaw("VerticalAim");
        Vector2 aimDirection = new Vector2(aimX, -aimY).normalized; // Normalisieren der Richtung
        if(aimDirection.magnitude > 0.1f)
        {
            aimDir = aimDirection;
        }
        else
        {
            aimDirection = aimDir;
        }
        frame.aimDirection = aimDirection;

        // Seil-Eingaben
        if (Input.GetButtonDown("Fire3"))
        {
            frame.swingButtonPressed = true;
        }
        if (Input.GetButton("Fire3"))
        {
            frame.swingButtonHeld = true;
        }
        if (Input.GetButtonUp("Fire3"))
        {
            frame.swingButtonReleased = true;
        }
        if(Input.GetAxisRaw("DpadHorizontal") > 0)
        {
            frame.upgradeButton1Pressed = true;
        }
        if(Input.GetAxisRaw("DpadHorizontal") < 0)
        {
            frame.upgradeButton2Pressed = true;
        }
        if(Input.GetAxisRaw("DpadVertical") > 0)
        {
            frame.upgradeButton3Pressed = true;
        }

        return frame;
    }
}

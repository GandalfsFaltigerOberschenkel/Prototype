using UnityEngine;
public interface IInputDevice
{
    public string Name { get; set; }
    public InputFrame ProcessInput();
}
public struct InputFrame
{

    public Vector2 inputDirection;
    public bool actionButtonPressed;
    public bool actionButtonHeld;
    public bool actionButtonReleased;
    public bool swingButtonPressed;
    public bool swingButtonHeld;
    public bool swingButtonReleased;
    public Vector2 aimDirection;
    public bool upgradeButton1Pressed;
    public bool upgradeButton2Pressed;
    public bool upgradeButton3Pressed;
}


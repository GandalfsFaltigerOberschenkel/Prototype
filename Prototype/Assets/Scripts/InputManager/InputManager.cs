using UnityEngine;

public class InputManager : MonoBehaviour
{
    private IInputDevice inputDevice;
    public InputFrame currentInputFrame;

    public void Initialize(IInputDevice initialDevice)
    {
        inputDevice = initialDevice;
    }

    public void GatherInput()
    {
        currentInputFrame = inputDevice.ProcessInput();
    }

    public void SetInputDevice(IInputDevice newDevice)
    {
        inputDevice = newDevice;
    }
}

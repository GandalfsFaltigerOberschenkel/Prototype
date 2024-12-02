using UnityEngine;

public class KeyboardDevice : IInputDevice
{
    public string Name { get; set; } = "Keyboard";
    public Transform playerPos;
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
        if (Input.GetKey(KeyCode.Space))
        {
            frame.actionButtonHeld = true;
        }
        frame.inputDirection = inputDirection;
        //Get the aim direction from the direction of the mouse cursor relative to the player
        if (playerPos != null)
        {
            var worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
            var facingDirection = worldMousePosition - playerPos.position;
            var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
            if (aimAngle < 0f)
            {
                aimAngle = Mathf.PI * 2 + aimAngle;
            }
            var aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;
            frame.aimDirection = aimDirection;
        }
        else
        {
            playerPos = GameObject.FindGameObjectWithTag("Player").transform;
            frame.aimDirection = Vector2.zero;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            frame.swingButtonPressed = true;
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            frame.swingButtonHeld = true;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            frame.swingButtonReleased = true;
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            frame.upgradeButton1Pressed = true;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            frame.upgradeButton2Pressed = true;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            frame.upgradeButton3Pressed = true;
        }
        return frame;
    }
}

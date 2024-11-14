using UnityEngine;

public class Player : MonoBehaviour
{
    public InputManager[] inputManagers;

    public void Update()
    {
        
        Vector3 movement = Vector3.zero;
        foreach (var inputManager in inputManagers)
        {
            inputManager.GatherInput();
            if (inputManager != null)
            {
                movement += new Vector3(inputManager.currentInputFrame.inputDirection.x, 0, 0) * 30 * Time.deltaTime;
            }
        }
        transform.position += movement;
    }
}

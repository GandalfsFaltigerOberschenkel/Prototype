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
    private void FailedAnimation()
    {
      
        
    }
    // RopeInputHandler.cs
    private void HandleInput()
    {
        Vector2 aimDirection = ropeSystem.player.input.aimDirection;
        RaycastHit2D hit;
        hit = Physics2D.Raycast(ropeSystem.player.transform.position, aimDirection, ropeSystem.ropeMaxCastDistance, ropeSystem.ropeLayerMask);
        if(hit.collider != null)
        {
            Debug.Log("Hit something");
            CursorChanger.Instance.SetCursor(1);
            ropeSystem.crosshairSprite.color = Color.green;
        }
        else
        {
            CursorChanger.Instance.SetCursor(0);
            ropeSystem.crosshairSprite.color = Color.red;
        }
        if (ropeSystem.player.input.swingButtonPressed)
        {
            ropeSystem.ropeSound.Play();
        }
        if (ropeSystem.player.input.swingButtonHeld)
        {
            Debug.Log("Swing button held");
            try { 
            ropeSystem.ropeStateManager.HandleSwingButtonHeld(aimDirection);
                // Sound abspielen, wenn die Schwingtaste gedrückt wird
            }
            catch (System.Exception e)
            {
                //Play Error Swing Animation
                FailedAnimation();
                
            }
        }

        if (ropeSystem.player.input.swingButtonReleased)
        {
            Debug.Log("Swing button released");
            ropeSystem.ropeStateManager.ResetRope();
        }
    }
}

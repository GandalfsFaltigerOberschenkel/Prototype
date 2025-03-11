using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    bool isFlipped = false;

    public void SetWalkingSpeed(float speed)
    {
        
        if(speed < 0)
        {
            isFlipped = false;
        }
        else if (speed == 0)
        {
           
        }
        else
        {
            isFlipped = true;
        }
        spriteRenderer.flipX = isFlipped;
        animator.SetFloat("Speed", Mathf.Abs(speed));
    }
    public void SetGrounded(bool isGrounded)
    {
        animator.SetBool("IsGrounded", isGrounded);
    }
    public void SetVerticalSpeed(float speed)
    {
        animator.SetFloat("ySpeed",speed);
    }
    public void SetFalling(bool isFalling)
    {
        animator.SetBool("IsFalling", isFalling);
    }
}

using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    
    public void SetWalkingSpeed(float speed)
    {
        
        if(speed < 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
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

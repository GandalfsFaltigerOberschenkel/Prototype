using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator;
    public void SetWalkingSpeed(float speed)
    {
        animator.SetFloat("Speed", speed);
    }
    public void SetJumping(bool isJumping)
    {
        animator.SetBool("IsJumping", isJumping);
    }
    public void SetFalling(bool isFalling)
    {
        animator.SetBool("IsFalling", isFalling);
    }
}

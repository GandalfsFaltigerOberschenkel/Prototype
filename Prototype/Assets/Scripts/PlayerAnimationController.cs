using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator;
    
    public void SetWalkingSpeed(float speed)
    {
        
        if(speed < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        animator.SetFloat("Speed", Mathf.Abs(speed));
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

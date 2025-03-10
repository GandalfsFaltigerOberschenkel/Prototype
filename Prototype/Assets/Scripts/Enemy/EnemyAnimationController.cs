using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    public Animator animator;
    public EnemyController enemyController;

    // Update is called once per frame
    void Update()
    {
        if(enemyController == null)
        {
            return;
        }
        EnemyState currentState = enemyController.currentState;
        switch(currentState)
        {
            case EnemyState.Idle:
                animator.SetBool("isIdle", true);
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", false);
                animator.SetBool("isTakingDamage", false);
                break;
            case EnemyState.Walking:
                animator.SetBool("isIdle", false);
                animator.SetBool("isWalking", true);
                animator.SetBool("isAttacking", false);
                animator.SetBool("isTakingDamage", false);
                break;
            case EnemyState.Attacking:
                animator.SetBool("isIdle", false);
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", true);
                animator.SetBool("isTakingDamage", false);
                break;
            case EnemyState.TakeDamage:
                animator.SetBool("isIdle", false);
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", false);
                animator.SetBool("isTakingDamage", true);
                break;
            case EnemyState.Dead:
                animator.SetBool("isIdle", false);
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", false);
                animator.SetBool("isTakingDamage", false);
                break;
        }
    }
}

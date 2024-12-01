using UnityEngine;

public class MeleeEnemyController : EnemyController
{
    protected override void HandleAttackingState()
    {
        movement.MoveDir(Vector2.zero);

        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            animator.SetTrigger("Attack");
            PushPlayerBack();
        }
        else
        {
            currentState = EnemyState.Walking;
        }
    }

    private void PushPlayerBack()
    {
        Vector2 pushDirection = (player.position - transform.position).normalized;
        player.GetComponent<Rigidbody2D>().AddForce(pushDirection * pushStrength, ForceMode2D.Impulse);
        StartCoroutine(player.GetComponent<FallThroughPlattforms>().FallThrough());
    }
}

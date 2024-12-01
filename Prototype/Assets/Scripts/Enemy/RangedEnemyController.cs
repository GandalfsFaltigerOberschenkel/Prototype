using System.Collections;
using UnityEngine;

public class RangedEnemyController : EnemyController
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;
    public float attackCooldown = 2f;
    bool canShoot = false;
    private IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(attackCooldown);
        canShoot = true;
    }
    protected override void Start()
    {
        base.Start();
        StartCoroutine(ShootCooldown());

    }
    protected override void HandleAttackingState()
    {
        movement.MoveDir(Vector2.zero);

        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            if (canShoot)
            {
                animator.SetTrigger("Attack");
                ShootProjectile();
                StartCoroutine(ShootCooldown());
            }

        }
        else
        {
            currentState = EnemyState.Walking;
        }
    }

    private void ShootProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Vector2 direction = (player.position - transform.position).normalized;
        projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * projectileSpeed;
        Destroy(projectile,2f);
    }
}

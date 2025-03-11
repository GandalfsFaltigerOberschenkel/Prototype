using System.Collections;
using UnityEngine;

public class RangedEnemyController : EnemyController
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;
    public float attackCooldown = 2f;
    public float attackIdle = 2f;
    bool canShoot = false;
    public AudioSource shootSound;
    bool isAttacking = false;
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
                if (!isAttacking)
                {
                    StartCoroutine(ShootProjectile());
                    StartCoroutine(ShootCooldown());
                }
            }

        }
        else
        {
            currentState = EnemyState.Walking;
        }
    }

    private IEnumerator ShootProjectile()
    { 
        bool currentFlip = GetComponent<SpriteRenderer>().flipX;
        //Flip the sprite based on the direction of the player
        if (player.position.x > transform.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (player.position.x < transform.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        isAttacking = true;
        animator.SetBool("isIdle", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("DelayAttack", true);

        yield return new WaitForSeconds(attackIdle);

        if (!stunned) // Check if the enemy is not stunned before continuing the attack
        {
            animator.SetBool("DelayAttack", false);
            animator.SetBool("isAttacking", true);

            shootSound.Play();
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Vector2 direction = (player.position - transform.position).normalized;
            projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * projectileSpeed;

            yield return new WaitForSeconds(1f); // Kurze Verzögerung, um sicherzustellen, dass die Animation abgespielt wird

            animator.SetBool("isAttacking", false);
        }

        isAttacking = false;
        GetComponent<SpriteRenderer>().flipX = currentFlip;
    }
}

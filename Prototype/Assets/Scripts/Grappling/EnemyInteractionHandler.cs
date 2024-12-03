using UnityEngine;

public class EnemyInteractionHandler : MonoBehaviour
{
    public int damage = 50;


    public void HandleEnemyHit(GameObject enemy)
    {
        var meleeEnemy = enemy.GetComponent<MeleeEnemyController>();
        var rangedEnemy = enemy.GetComponent<RangedEnemyController>();

        if (meleeEnemy != null)
        {
            meleeEnemy.TakeDamage(damage);
            meleeEnemy.OnEnemyDestroyed += HandleEnemyDestroyed;
        }
        else if (rangedEnemy != null)
        {
            rangedEnemy.TakeDamage(damage);
            rangedEnemy.OnEnemyDestroyed += HandleEnemyDestroyed;
        }
    }

    private void HandleEnemyDestroyed(EnemyController enemy)
    {
        // Handle enemy destruction logic here
    }
}

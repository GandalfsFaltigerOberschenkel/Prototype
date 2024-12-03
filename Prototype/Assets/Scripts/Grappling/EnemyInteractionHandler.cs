using UnityEngine;

public class EnemyInteractionHandler : MonoBehaviour
{
    public int damage = 50;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

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

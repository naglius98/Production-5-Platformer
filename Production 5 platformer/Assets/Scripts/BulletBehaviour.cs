using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public int BulletDamage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyBehaviour enemy = collision.GetComponent<EnemyBehaviour>();

        if (enemy)
        {
            enemy.TakeDamage(BulletDamage);
            Destroy(gameObject);
        }
    }
}

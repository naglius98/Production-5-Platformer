using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
   public int BulletDamage = 1;
    public bool ShotByPlayer = true; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ShotByPlayer)
        {
            // Player bullets only damage enemies
            EnemyBehaviour enemy = collision.GetComponent<EnemyBehaviour>();
            if (enemy)
            {
                enemy.TakeDamage(BulletDamage);
                Destroy(gameObject);
            }
        }
        else
        {
            // Enemy bullets only damage the player
            PlayerHealth player = collision.GetComponent<PlayerHealth>();
            if (player)
            {
                player.TakeDamage(BulletDamage);
                Destroy(gameObject);
            }
        }
    }
}

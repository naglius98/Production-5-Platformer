using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public int BulletDamage = 1;
    public bool ShotByPlayer = true;
    private bool hasHit = false; // Prevent multiple hits

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Prevent bullet from dealing damage twice
        if (hasHit) return;

        if (ShotByPlayer)
        {
            // Player bullets only damage enemies
            EnemyBehaviour enemy = collision.GetComponent<EnemyBehaviour>();
            if (enemy)
            {
                hasHit = true;
                Debug.Log("Bullet hit enemy, dealing " + BulletDamage + " damage");
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
                hasHit = true;
                Debug.Log("Bullet hit player, dealing " + BulletDamage + " damage");
                player.TakeDamage(BulletDamage);
                Destroy(gameObject);
            }
        }
    }
}

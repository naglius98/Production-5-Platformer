using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
   public GameObject BulletPrefab;
    public float BulletSpeed = 40.0f;
    public AudioClip ShootSound;
    public float ShootCooldown = 1.0f;
    
    private EnemyBehaviour enemyBehaviour;
    private AudioSource audioSource;
    private float lastShootTime = 0f;
    private Transform player;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Get reference to the enemy behaviour script
        enemyBehaviour = GetComponent<EnemyBehaviour>();
        
        // Find the player in the scene
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        // Only shoot if we have references and the enemy can see the player
        if (player != null && enemyBehaviour != null && enemyBehaviour.CanSeePlayer && CanShoot())
        {
            Shoot();
        }
    }

    bool CanShoot()
    {
        return Time.time >= lastShootTime + ShootCooldown;
    }

    void Shoot()
    {
        lastShootTime = Time.time;

        // Play shoot sound
        if (ShootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(ShootSound);
        }

        // Direction towards player
        Vector3 ShootDirection = (player.position - transform.position).normalized;
        
        // Spawn bullet slightly in front of enemy to avoid self-collision
        Vector3 spawnPosition = transform.position + ShootDirection * 0.5f;

        GameObject Bullet = Instantiate(BulletPrefab, spawnPosition, Quaternion.identity);
        
        // Mark this bullet as shot by enemy
        BulletBehaviour bulletScript = Bullet.GetComponent<BulletBehaviour>();
        if (bulletScript != null)
        {
            bulletScript.ShotByPlayer = false;
        }
        
        Bullet.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(ShootDirection.x, ShootDirection.y) * BulletSpeed;
        Destroy(Bullet, 2.0f);
    }
}

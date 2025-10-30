using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject BulletPrefab;
    public float BulletSpeed = 40.0f;
    public AudioClip ShootSound;
    
    private AudioSource audioSource;
    private float lastShootTime = 0f;
    public float ShootCooldown = 1.0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && CanShoot())
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

        // Mouse position
        Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Direction
        Vector3 ShootDirection = (MousePosition - transform.position).normalized;

        GameObject Bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
        Bullet.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(ShootDirection.x, ShootDirection.y) * BulletSpeed;
        Destroy(Bullet, 2.0f);
    }
}

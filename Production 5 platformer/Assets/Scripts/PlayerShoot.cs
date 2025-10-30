using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject BulletPrefab;
    public float BulletSpeed = 40.0f;
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Mouse position
        Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Direction
        Vector3 ShootDirection = (MousePosition - transform.position).normalized;

        GameObject Bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
        Bullet.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(ShootDirection.x, ShootDirection.y) * BulletSpeed;
        Destroy(Bullet, 2.0f);
    }
}

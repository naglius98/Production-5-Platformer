using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int MaxHealth = 3;
    private int CurrentHealth;

    public HealthBehaviour HealthBar;
    private SpriteRenderer SpriteRenderer;
    private Color OriginalColor;

    void Start()
    {
        CurrentHealth = MaxHealth;
        HealthBar.SetMaxHearts(MaxHealth);

        SpriteRenderer = GetComponent<SpriteRenderer>();
        OriginalColor = SpriteRenderer.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyBehaviour enemy = collision.GetComponent<EnemyBehaviour>();

        if (enemy)
        {
            TakeDamage(enemy.Damage);
        }
    }

    private void TakeDamage(int Damage)
    {
        CurrentHealth -= Damage;
        HealthBar.UpdateHearts(CurrentHealth);

        StartCoroutine(FlashRed());

        if (CurrentHealth <= 0)
        {
            // Game over
        }
    }

    private IEnumerator FlashRed()
    {
        SpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.25f);
        SpriteRenderer.color = OriginalColor;
    }
}

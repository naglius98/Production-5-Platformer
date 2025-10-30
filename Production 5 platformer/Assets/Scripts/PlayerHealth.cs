using UnityEngine;
using System.Collections;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int MaxHealth = 3;
    private int CurrentHealth;

    public HealthBehaviour HealthBar;
    private SpriteRenderer SpriteRenderer;
    private Color OriginalColor;

    // event for game over
    public static event Action OnPlayerDeath;
    

    void Start()
    {
        CurrentHealth = MaxHealth;
        HealthBar.SetMaxHearts(MaxHealth);

        SpriteRenderer = GetComponent<SpriteRenderer>();
        OriginalColor = SpriteRenderer.color;
    }

    public void ResetPlayer()
    {
        CurrentHealth = MaxHealth;
        HealthBar.SetMaxHearts(MaxHealth);
        
        // Resume time when restarting
        Time.timeScale = 1f;
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
            // Game over - freeze everything
            Time.timeScale = 0f;
            OnPlayerDeath?.Invoke();
        }
    }

    private IEnumerator FlashRed()
    {
        SpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.25f);
        SpriteRenderer.color = OriginalColor;
    }
}

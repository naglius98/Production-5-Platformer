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

    [Header("Camera Shake")]
    public float ShakeDuration = 0.2f;
    public float ShakeMagnitude = 0.15f;

    private CameraShake cameraShake;

    // event for game over
    public static event Action OnPlayerDeath;
    

    void Start()
    {
        CurrentHealth = MaxHealth;
        HealthBar.SetMaxHearts(MaxHealth);

        SpriteRenderer = GetComponent<SpriteRenderer>();
        OriginalColor = SpriteRenderer.color;

        // Find the camera shake component
        cameraShake = Camera.main.GetComponent<CameraShake>();
        if (cameraShake == null)
        {
            Debug.LogWarning("CameraShake script not found on Main Camera!");
        }
    }

    public void ResetPlayer()
    {
        CurrentHealth = MaxHealth;
        HealthBar.SetMaxHearts(MaxHealth);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyBehaviour enemy = collision.GetComponent<EnemyBehaviour>();

        if (enemy)
        {
            TakeDamage(enemy.Damage);
        }
    }

    
    public void TakeDamage(int Damage)
    {
        CurrentHealth -= Damage;
        HealthBar.UpdateHearts(CurrentHealth);

        // Trigger camera shake
        if (cameraShake != null)
        {
            cameraShake.Shake(ShakeDuration, ShakeMagnitude);
        }

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

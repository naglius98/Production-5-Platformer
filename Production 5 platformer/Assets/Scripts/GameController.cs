using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
   int ProgressScore;

   public Slider ProgressBar;

   public GameObject GameOverScreen;
   
   public PlayerHealth playerHealth;
   public PlayerMovement playerMovement;
   public Spawner spawner;

   void Start()
   {
        ProgressScore = 0;
        ProgressBar.value = 0;
        GemPickup.OnGemCollected += IncreaseScoreAmount;

        // Death event
        PlayerHealth.OnPlayerDeath += ShowGameOverScreen;
        GameOverScreen.SetActive(false);
   }

   void IncreaseScoreAmount(int amount)
   {
        ProgressScore += amount;
        ProgressBar.value = ProgressScore;
        Debug.Log("Score: " + ProgressScore);

        if (ProgressScore >= 100) // if our score is greater than 100, we win
        {
            Debug.Log("You win!");
        }
   }

   void ShowGameOverScreen()
   {
        GameOverScreen.SetActive(true);
   }

   public void RestartGame()
   {
        // Hide game over screen
        GameOverScreen.SetActive(false);
        
        // Reset score
        ProgressScore = 0;
        ProgressBar.value = 0;
        
        // Reset player health 
        if (playerHealth != null)
        {
            playerHealth.ResetPlayer();
        }
        
        // Reset player movement
        if (playerMovement != null)
        {
            playerMovement.ResetPlayer();
        }
        
        // Reset spawner 
        if (spawner != null)
        {
            spawner.ResetSpawner();
        }
   }
}

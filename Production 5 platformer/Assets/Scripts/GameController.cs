using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
   int ProgressScore;

   public Slider ProgressBar;

   public GameObject GameOverScreen;
   public GameObject YouWinScreen;
   
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
        YouWinScreen.SetActive(false);
   }

   void IncreaseScoreAmount(int amount)
   {
        ProgressScore += amount;
        ProgressBar.value = ProgressScore;
        Debug.Log("Score: " + ProgressScore);

        if (ProgressScore >= 50) // if our score is greater than 50, we win
        {
            ShowYouWinScreen();
        }
   }

   void ShowGameOverScreen()
   {
        Time.timeScale = 0f;
        GameOverScreen.SetActive(true);
        MusicManager.PauseMusic();
   }

   void ShowYouWinScreen()
   {
        Time.timeScale = 0f;
        YouWinScreen.SetActive(true);
        MusicManager.PauseMusic();
   }

   public void RestartGame() 
   {
        // Resume time first
        Time.timeScale = 1f;
        
        // Hide all game screens
        GameOverScreen.SetActive(false);
        YouWinScreen.SetActive(false);
        
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

        // Resume music
        MusicManager.PlayMusic(true);
   }
}

using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
   int ProgressScore;

   public Slider ProgressBar;

   void Start()
   {
        ProgressScore = 0;
        ProgressBar.value = 0;
        GemPickup.OnGemCollected += IncreaseScoreAmount;
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
}

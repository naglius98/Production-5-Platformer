using UnityEngine;
using UnityEngine.UI;   
using System.Collections.Generic;

public class HealthBehaviour : MonoBehaviour
{
   public Image HeartPrefab;
   public Sprite FullHeart;
   public Sprite EmptyHeart;

   private List<Image> Hearts = new List<Image>();

   public void SetMaxHearts(int maxHearts)
   {
        foreach (var heart in Hearts)
        {
            Destroy(heart.gameObject);  
        }

        Hearts.Clear();

        for (int i = 0; i < maxHearts; i++)
        {
            Image newHeart = Instantiate(HeartPrefab, transform);
            newHeart.sprite = FullHeart;
            newHeart.color = Color.red;
            Hearts.Add(newHeart);
        }
   }

   public void UpdateHearts(int currentHearts)
   {
        for (int i = 0; i < Hearts.Count; i++)
        {
            if (i < currentHearts) 
            {
                Hearts[i].sprite = FullHeart;
            }
            else
            {
                Hearts[i].sprite = EmptyHeart;
            }
        }
    }
}

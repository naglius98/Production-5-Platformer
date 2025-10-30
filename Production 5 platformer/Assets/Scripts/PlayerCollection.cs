using UnityEngine;

public class PlayerCollection : MonoBehaviour
{
   private AudioSource audioSource;
   public AudioClip GemPickupSound;

   void Start()
   {
        audioSource = GetComponent<AudioSource>();
   }

   private void OnTriggerEnter2D(Collider2D collision)
   {
        ItemInterface item = collision.GetComponent<ItemInterface>();

        if (item != null)
        {
            // Play pickup sound if it's a gem
            GemPickup gem = collision.GetComponent<GemPickup>();
            if (gem != null && GemPickupSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(GemPickupSound);
            }
            
            item.Collect();
        }
    }
}

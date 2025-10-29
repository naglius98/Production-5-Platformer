using UnityEngine;

public class PlayerCollection : MonoBehaviour
{
   private void OnTriggerEnter2D(Collider2D collision)
   {

        ItemInterface item = collision.GetComponent<ItemInterface>();

        if (item != null)
        {
            item.Collect();
        }
    }
}

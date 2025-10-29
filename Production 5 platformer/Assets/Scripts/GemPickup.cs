using UnityEngine;

public class GemPickup : MonoBehaviour, ItemInterface
{
    public void Collect()
    {
        Destroy(gameObject);
        Debug.Log("Gem collected");
    }
}

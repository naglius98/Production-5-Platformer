using UnityEngine;
using System;

public class GemPickup : MonoBehaviour, ItemInterface
{
    public static event Action<int> OnGemCollected;
    public int GemValue = 5;
        public void Collect()
    {
        OnGemCollected.Invoke(GemValue);
        Destroy(gameObject);
        Debug.Log("Gem collected");
    }
}

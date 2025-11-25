using UnityEngine;

public class CollectibleFactory
{
    public Collectible SpawnCollectible(Collectible prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (prefab == null)
        {
            Debug.LogError("CollectibleFactory: Cannot spawn null prefab!");
            return null;
        }
        
        Collectible collectible = Object.Instantiate(prefab, position, rotation, parent);
        return collectible;
    }
}

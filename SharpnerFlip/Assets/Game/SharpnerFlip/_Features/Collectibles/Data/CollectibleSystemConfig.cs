using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectibleSystemConfig", menuName = "Game/Collectible System Config")]
public class CollectibleSystemConfig : ScriptableObject
{
    [Header("Collectible Prefabs Database")]
    public List<Collectible> collectiblePrefabs = new List<Collectible>();
    
    [Header("Spawn Settings")]
    public Vector3 initialSpawnPosition = new Vector3(0f, 0f, 10f);
    public Vector3 spawnDirection = new Vector3(0f, 0f, 1f);
    public float spaceBetweenCollectibles = 5f;
    
    public Collectible GetPrefabByType(CollectibleType type)
    {
        foreach (Collectible prefab in collectiblePrefabs)
        {
            if (prefab.collectibleType == type)
            {
                return prefab;
            }
        }
        
        Debug.LogWarning($"CollectibleSystemConfig: No prefab found for type {type}");
        return null;
    }
}

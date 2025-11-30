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
    
    [Header("Sharpening Settings")]
    public float sharpeningSpeed = 2f;
    
    [Header("Environment Settings")]
    public GameObject environmentPrefab;
    public Vector3 environmentInitialPosition = new Vector3(0f, 0f, 0f);
    public Vector3 environmentRotation = new Vector3(0f, 0f, 0f);
    public float environmentPieceLength = 40f;
    public float environmentSpawnPadding = 20f;
    
    [Header("Environment Culling")]
    public int activeEnvironmentRange = 1;
    
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

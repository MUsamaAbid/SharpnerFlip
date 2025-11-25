using System.Collections.Generic;
using UnityEngine;

public class CollectibleSystemController
{
    private Transform collectiblesParent;
    private CollectibleFactory factory;
    private CollectibleSystemConfig config;
    private List<Collectible> spawnedCollectibles = new List<Collectible>();
    
    public void Initialize(Transform parent, CollectibleSystemConfig systemConfig)
    {
        collectiblesParent = parent;
        config = systemConfig;
        factory = new CollectibleFactory();
    }
    
    public void SpawnCollectibles(LevelData levelData)
    {
        if (levelData == null)
        {
            Debug.LogError("CollectibleSystemController: LevelData is null!");
            return;
        }
        
        if (config == null)
        {
            Debug.LogError("CollectibleSystemController: CollectibleSystemConfig is null! Call Initialize() first.");
            return;
        }
        
        if (levelData.collectibleTypes.Count == 0)
        {
            Debug.LogWarning("CollectibleSystemController: No collectible types assigned in LevelData!");
            return;
        }
        
        if (factory == null)
        {
            Debug.LogError("CollectibleSystemController: Not initialized! Call Initialize() first.");
            return;
        }
        
        Vector3 currentSpawnPosition = config.initialSpawnPosition;
        Vector3 spawnOffset = config.spawnDirection.normalized * config.spaceBetweenCollectibles;
        
        for (int i = 0; i < levelData.totalCollectibles; i++)
        {
            CollectibleType type = GetNextType(levelData, i);
            Collectible prefab = config.GetPrefabByType(type);
            
            if (prefab == null)
            {
                Debug.LogWarning($"Skipping {type} - no prefab found in config!");
                continue;
            }
            
            Collectible spawned = factory.SpawnCollectible(
                prefab, 
                currentSpawnPosition, 
                Quaternion.identity, 
                collectiblesParent
            );
            
            if (spawned != null)
            {
                spawnedCollectibles.Add(spawned);
                Debug.Log($"Spawned {spawned.collectibleType} at {currentSpawnPosition}");
            }
            
            currentSpawnPosition += spawnOffset;
        }
        
        Debug.Log($"Level {levelData.levelNumber}: Spawned {spawnedCollectibles.Count} collectibles");
    }
    
    private CollectibleType GetNextType(LevelData levelData, int index)
    {
        if (levelData.randomizeOrder)
        {
            int randomIndex = Random.Range(0, levelData.collectibleTypes.Count);
            return levelData.collectibleTypes[randomIndex];
        }
        else
        {
            int typeIndex = index % levelData.collectibleTypes.Count;
            return levelData.collectibleTypes[typeIndex];
        }
    }
    
    public void ClearAllCollectibles()
    {
        foreach (Collectible collectible in spawnedCollectibles)
        {
            if (collectible != null)
            {
                Object.Destroy(collectible.gameObject);
            }
        }
        spawnedCollectibles.Clear();
    }
    
    public List<Collectible> GetSpawnedCollectibles()
    {
        return spawnedCollectibles;
    }
    
    public int GetCollectedCount()
    {
        int count = 0;
        foreach (Collectible collectible in spawnedCollectibles)
        {
            if (collectible == null)
                count++;
        }
        return count;
    }
}

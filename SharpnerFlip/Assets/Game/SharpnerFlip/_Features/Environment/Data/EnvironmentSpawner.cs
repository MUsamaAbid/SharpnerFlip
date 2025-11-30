using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpawner
{
    private Transform environmentParent;
    private List<GameObject> spawnedEnvironments = new List<GameObject>();
    private CollectibleSystemConfig config;
    private int currentActiveIndex = -1;
    
    public void Initialize(Transform parent)
    {
        environmentParent = parent;
    }
    
    public void SpawnEnvironment(CollectibleSystemConfig systemConfig, LevelData levelData)
    {
        config = systemConfig;
        
        if (config.environmentPrefab == null)
        {
            Debug.LogWarning("EnvironmentSpawner: No environment prefab assigned!");
            return;
        }
        
        float totalDistanceNeeded = CalculateTotalDistance(config, levelData);
        int piecesNeeded = CalculatePiecesNeeded(totalDistanceNeeded, config.environmentPieceLength, config.environmentSpawnPadding);
        
        Debug.Log($"Environment Spawner: Total distance needed = {totalDistanceNeeded}, Pieces to spawn = {piecesNeeded}");
        
        SpawnEnvironmentPieces(config, piecesNeeded);
        DisableAllEnvironments();
        currentActiveIndex = -1;
    }
    
    private float CalculateTotalDistance(CollectibleSystemConfig config, LevelData levelData)
    {
        float distanceBetweenCollectibles = config.spaceBetweenCollectibles;
        int totalCollectibles = levelData.totalCollectibles;
        
        float collectiblesDistance = (totalCollectibles - 1) * distanceBetweenCollectibles;
        
        float startPosition = config.initialSpawnPosition.z;
        float endPosition = startPosition + collectiblesDistance;
        
        float totalDistance = endPosition + config.environmentSpawnPadding;
        
        return totalDistance;
    }
    
    private int CalculatePiecesNeeded(float totalDistance, float pieceLength, float padding)
    {
        if (pieceLength <= 0f)
        {
            Debug.LogError("EnvironmentSpawner: Environment piece length must be greater than 0!");
            return 0;
        }
        
        float distanceWithPadding = totalDistance + padding;
        int pieces = Mathf.CeilToInt(distanceWithPadding / pieceLength);
        
        return Mathf.Max(1, pieces);
    }
    
    private void SpawnEnvironmentPieces(CollectibleSystemConfig config, int piecesCount)
    {
        Vector3 currentPosition = config.environmentInitialPosition;
        Vector3 spawnOffset = config.spawnDirection.normalized * config.environmentPieceLength;
        Quaternion rotation = Quaternion.Euler(config.environmentRotation);
        
        for (int i = 0; i < piecesCount; i++)
        {
            GameObject piece = Object.Instantiate(
                config.environmentPrefab,
                currentPosition,
                rotation,
                environmentParent
            );
            
            piece.name = $"Environment_Piece_{i}";
            spawnedEnvironments.Add(piece);
            
            Debug.Log($"Spawned environment piece {i} at {currentPosition}");
            
            currentPosition += spawnOffset;
        }
    }
    
    public void ClearAllEnvironments()
    {
        foreach (GameObject env in spawnedEnvironments)
        {
            if (env != null)
            {
                Object.Destroy(env);
            }
        }
        spawnedEnvironments.Clear();
        currentActiveIndex = -1;
    }
    
    public int GetSpawnedCount()
    {
        return spawnedEnvironments.Count;
    }
    
    public void UpdateActiveEnvironments(Vector3 sharpnerPosition)
    {
        if (spawnedEnvironments.Count == 0 || config == null)
            return;
        
        int targetIndex = CalculateEnvironmentIndex(sharpnerPosition);
        
        if (targetIndex == currentActiveIndex)
            return;
        
        currentActiveIndex = targetIndex;
        
        for (int i = 0; i < spawnedEnvironments.Count; i++)
        {
            if (spawnedEnvironments[i] == null)
                continue;
            
            int distance = Mathf.Abs(i - currentActiveIndex);
            bool shouldBeActive = distance <= config.activeEnvironmentRange;
            
            if (spawnedEnvironments[i].activeSelf != shouldBeActive)
            {
                spawnedEnvironments[i].SetActive(shouldBeActive);
            }
        }
    }
    
    private int CalculateEnvironmentIndex(Vector3 position)
    {
        float relativePosition = position.z - config.environmentInitialPosition.z;
        int index = Mathf.FloorToInt(relativePosition / config.environmentPieceLength);
        return Mathf.Clamp(index, 0, spawnedEnvironments.Count - 1);
    }
    
    private void DisableAllEnvironments()
    {
        foreach (GameObject env in spawnedEnvironments)
        {
            if (env != null)
            {
                env.SetActive(false);
            }
        }
    }
}

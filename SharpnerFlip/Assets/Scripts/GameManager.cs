using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("System Configuration")]
    [SerializeField] private CollectibleSystemConfig collectibleConfig;
    
    [Header("Level Configuration")]
    [SerializeField] private LevelData currentLevel;
    
    [Header("Hierarchy")]
    [SerializeField] private Transform collectiblesParent;
    
    private CollectibleSystemController collectibleController;
    
    private void Start()
    {
        InitializeGame();
    }
    
    private void InitializeGame()
    {
        if (collectibleConfig == null)
        {
            Debug.LogError("GameManager: No CollectibleSystemConfig assigned!");
            return;
        }
        
        if (currentLevel == null)
        {
            Debug.LogError("GameManager: No LevelData assigned!");
            return;
        }
        
        if (collectiblesParent == null)
        {
            GameObject parentObject = new GameObject("Collectibles");
            collectiblesParent = parentObject.transform;
        }
        
        collectibleController = new CollectibleSystemController();
        collectibleController.Initialize(collectiblesParent, collectibleConfig);
        collectibleController.SpawnCollectibles(currentLevel);
        
        Debug.Log($"Started {currentLevel.levelName}");
    }
    
    public void LoadLevel(LevelData levelData)
    {
        if (collectibleController != null)
        {
            collectibleController.ClearAllCollectibles();
        }
        
        currentLevel = levelData;
        collectibleController.SpawnCollectibles(currentLevel);
        Debug.Log($"Loaded {currentLevel.levelName}");
    }
    
    public void RestartLevel()
    {
        if (collectibleController != null && currentLevel != null)
        {
            collectibleController.ClearAllCollectibles();
            collectibleController.SpawnCollectibles(currentLevel);
            Debug.Log($"Restarted {currentLevel.levelName}");
        }
    }
    
    public bool CheckWinCondition()
    {
        if (collectibleController == null || currentLevel == null)
            return false;
        
        int collectedCount = collectibleController.GetCollectedCount();
        return collectedCount >= currentLevel.collectiblesRequiredToWin;
    }
}

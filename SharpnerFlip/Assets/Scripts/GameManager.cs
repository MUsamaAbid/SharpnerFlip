using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("System Configuration")]
    [SerializeField] private CollectibleSystemConfig collectibleConfig;
    
    [Header("Level Configuration")]
    [SerializeField] private LevelData currentLevel;
    
    [Header("Hierarchy")]
    [SerializeField] private Transform collectiblesParent;
    [SerializeField] private Sharpner sharpner;
    
    [Header("UI")]
    [SerializeField] private GameplayUIManager uiManager;
    
    private CollectibleSystemController collectibleController;
    private bool isGameOver;
    private string gameOverReason;
    
    private void Start()
    {
        InitializeGame();
        SetupSharpnerEvents();
    }
    
    private void InitializeGame()
    {
        isGameOver = false;
        gameOverReason = "";
        
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
        
        if (sharpner == null)
        {
            sharpner = FindFirstObjectByType<Sharpner>();
        }
        
        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<GameplayUIManager>();
        }
        
        collectibleController = new CollectibleSystemController();
        collectibleController.Initialize(collectiblesParent, collectibleConfig);
        collectibleController.SpawnCollectibles(currentLevel);
        
        Debug.Log($"Started {currentLevel.levelName}");
    }
    
    private void SetupSharpnerEvents()
    {
        if (sharpner != null)
        {
            sharpner.OnGameOver.AddListener(HandleGameOver);
            sharpner.OnCollectibleSharpened.AddListener(HandleCollectibleSharpened);
            sharpner.OnGroundHit.AddListener(() => gameOverReason = "Hit the ground!");
            sharpner.OnUnsharpenableHit.AddListener((collectible) => gameOverReason = $"Hit {collectible.collectibleType}!");
        }
    }
    
    private void HandleGameOver()
    {
        if (isGameOver)
            return;
        
        isGameOver = true;
        Debug.Log("=== GAME OVER ===");
        
        if (uiManager != null)
        {
            uiManager.ShowGameOverScreen(gameOverReason);
        }
        else
        {
            Debug.Log("Press R to restart or wait 3 seconds for auto-restart");
            Invoke(nameof(RestartLevel), 3f);
        }
    }
    
    private void HandleCollectibleSharpened(Collectible collectible)
    {
        Debug.Log($"Collectible sharpened: {collectible.collectibleType}");
        
        if (CheckWinCondition())
        {
            HandleLevelComplete();
        }
    }
    
    private void HandleLevelComplete()
    {
        Debug.Log($"=== LEVEL COMPLETE! ===");
        Debug.Log($"You sharpened all collectibles in {currentLevel.levelName}!");
        
        if (uiManager != null)
        {
            uiManager.ShowLevelCompleteScreen(currentLevel.levelName);
        }
    }
    
    private void Update()
    {
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public bool CheckWinCondition()
    {
        if (collectibleController == null || currentLevel == null)
            return false;
        
        int collectedCount = collectibleController.GetCollectedCount();
        return collectedCount >= currentLevel.collectiblesRequiredToWin;
    }
}

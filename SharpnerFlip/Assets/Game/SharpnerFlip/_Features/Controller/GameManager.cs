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
    [SerializeField] private Transform environmentParent;
    [SerializeField] private Sharpner sharpner;
    
    [Header("UI")]
    [SerializeField] private GameplayUIManager uiManager;
    
    [Header("Score")]
    [SerializeField] private ScoreManager scoreManager;
    
    private CollectibleSystemController collectibleController;
    private EnvironmentSpawner environmentSpawner;
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
        
        if (environmentParent == null)
        {
            GameObject parentObject = new GameObject("Environment");
            environmentParent = parentObject.transform;
        }
        
        if (sharpner == null)
        {
            sharpner = FindFirstObjectByType<Sharpner>();
        }
        
        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<GameplayUIManager>();
        }
        
        if (scoreManager == null)
        {
            scoreManager = FindFirstObjectByType<ScoreManager>();
            if (scoreManager == null)
            {
                GameObject scoreObject = new GameObject("ScoreManager");
                scoreManager = scoreObject.AddComponent<ScoreManager>();
            }
        }
        
        scoreManager.ResetScore();
        
        collectibleController = new CollectibleSystemController();
        collectibleController.Initialize(collectiblesParent, collectibleConfig);
        collectibleController.SpawnCollectibles(currentLevel);
        
        SetupFinishLine();
        
        environmentSpawner = new EnvironmentSpawner();
        environmentSpawner.Initialize(environmentParent);
        environmentSpawner.SpawnEnvironment(collectibleConfig, currentLevel);
        
        Debug.Log($"Started {currentLevel.levelName}");
    }
    
    private void SetupFinishLine()
    {
        FinishLine finishLine = collectibleController.GetFinishLine();
        if (finishLine != null)
        {
            finishLine.OnFinishLineCrossed += HandleLevelComplete;
            Debug.Log("Finish line event connected!");
        }
        else
        {
            Debug.LogWarning("No finish line found!");
        }
    }
    
    private void SetupSharpnerEvents()
    {
        if (sharpner != null)
        {
            sharpner.OnGameOver.AddListener(HandleGameOver);
            sharpner.OnGroundHit.AddListener(() => gameOverReason = "Hit the ground!");
            sharpner.OnUnsharpenableHit.AddListener((collectible) => gameOverReason = $"Hit {collectible.collectibleType}!");
            
            if (scoreManager != null)
            {
                sharpner.OnSharpeningStarted.AddListener(() => scoreManager.StartSharpening());
                sharpner.OnSharpeningStopped.AddListener(() => scoreManager.StopSharpening());
            }
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
    
    private void HandleLevelComplete()
    {
        if (isGameOver)
            return;
        
        isGameOver = true;
        Debug.Log($"=== LEVEL COMPLETE! ===");
        Debug.Log($"Finished {currentLevel.levelName}!");
        
        if (scoreManager != null)
        {
            scoreManager.StopSharpening();
        }
        
        if (uiManager != null)
        {
            uiManager.ShowLevelCompleteScreen(currentLevel.levelName);
        }
    }
    
    private void Update()
    {
        if (!isGameOver && sharpner != null && environmentSpawner != null)
        {
            environmentSpawner.UpdateActiveEnvironments(sharpner.transform.position);
        }
        
        // if (isGameOver && Input.GetKeyDown(KeyCode.R))
        // {
        //     RestartLevel();
        // }
    }
    
    public void LoadLevel(LevelData levelData)
    {
        if (collectibleController != null)
        {
            collectibleController.ClearAllCollectibles();
        }
        
        if (environmentSpawner != null)
        {
            environmentSpawner.ClearAllEnvironments();
        }
        
        currentLevel = levelData;
        collectibleController.SpawnCollectibles(currentLevel);
        environmentSpawner.SpawnEnvironment(collectibleConfig, currentLevel);
        Debug.Log($"Loaded {currentLevel.levelName}");
    }
    
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayUIManager : MonoBehaviour
{
    [Header("UI Screens")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject levelCompleteScreen;
    
    [Header("Game Over Elements")]
    [SerializeField] private TextMeshProUGUI gameOverReasonText;
    [SerializeField] private Button restartButton;
    
    [Header("Level Complete Elements")]
    [SerializeField] private TextMeshProUGUI levelCompleteText;
    [SerializeField] private Button nextLevelButton;
    
    private void Awake()
    {
        HideAllScreens();
        SetupButtons();
    }
    
    private void SetupButtons()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        }
        
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(OnNextLevelButtonClicked);
        }
    }
    
    public void ShowGameOverScreen(string reason = "")
    {
        HideAllScreens();
        
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            
            if (gameOverReasonText != null && !string.IsNullOrEmpty(reason))
            {
                gameOverReasonText.text = reason;
            }
        }
    }
    
    public void ShowLevelCompleteScreen(string levelName = "")
    {
        HideAllScreens();
        
        if (levelCompleteScreen != null)
        {
            levelCompleteScreen.SetActive(true);
            
            if (levelCompleteText != null && !string.IsNullOrEmpty(levelName))
            {
                levelCompleteText.text = $"Level Complete!\n{levelName}";
            }
        }
    }
    
    public void HideAllScreens()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }
        
        if (levelCompleteScreen != null)
        {
            levelCompleteScreen.SetActive(false);
        }
    }
    
    private void OnRestartButtonClicked()
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.RestartLevel();
        }
    }
    
    private void OnNextLevelButtonClicked()
    {
        Debug.Log("Next level not implemented yet");
    }
}

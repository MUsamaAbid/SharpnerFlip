using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayUIManager : MonoBehaviour
{
    [Header("UI Screens")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject levelCompleteScreen;
    
    [Header("Gameplay HUD")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    
    [Header("Game Over Elements")]
    [SerializeField] private TextMeshProUGUI gameOverReasonText;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private TextMeshProUGUI gameOverHighScoreText;
    [SerializeField] private GameObject newHighScoreBanner;
    [SerializeField] private Button restartButton;
    
    [Header("Level Complete Elements")]
    [SerializeField] private TextMeshProUGUI levelCompleteText;
    [SerializeField] private TextMeshProUGUI levelCompleteScoreText;
    [SerializeField] private Button nextLevelButton;
    
    private ScoreManager scoreManager;
    
    private void Awake()
    {
        HideAllScreens();
        SetupButtons();
    }
    
    private void Start()
    {
        scoreManager = FindFirstObjectByType<ScoreManager>();
        
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged += UpdateScore;
            scoreManager.OnComboChanged += UpdateCombo;
            scoreManager.OnNewHighScore += OnNewHighScore;
            
            if (scoreManager.ScoreData != null)
            {
                UpdateHighScore(scoreManager.ScoreData.AllTimeHighScore);
            }
        }
        
        UpdateScore(0);
        UpdateCombo(1f);
        
        if (newHighScoreBanner != null)
        {
            newHighScoreBanner.SetActive(false);
        }
    }
    
    private void OnDestroy()
    {
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged -= UpdateScore;
            scoreManager.OnComboChanged -= UpdateCombo;
            scoreManager.OnNewHighScore -= OnNewHighScore;
        }
    }
    
    private void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
    
    private void UpdateCombo(float combo)
    {
        if (comboText != null)
        {
            if (combo > 1f)
            {
                comboText.text = $"x{combo:F1}";
                comboText.gameObject.SetActive(true);
            }
            else
            {
                comboText.gameObject.SetActive(false);
            }
        }
    }
    
    private void UpdateHighScore(int highScore)
    {
        if (highScoreText != null)
        {
            highScoreText.text = $"Best: {highScore}";
        }
    }
    
    private void OnNewHighScore(int newHighScore)
    {
        UpdateHighScore(newHighScore);
        
        if (newHighScoreBanner != null)
        {
            newHighScoreBanner.SetActive(true);
        }
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
            
            if (scoreManager != null)
            {
                int finalScore = scoreManager.GetFinalScore();
                
                if (gameOverScoreText != null)
                {
                    gameOverScoreText.text = $"Score: {finalScore}";
                }
                
                if (gameOverHighScoreText != null && scoreManager.ScoreData != null)
                {
                    bool isNewHigh = scoreManager.ScoreData.IsNewHighScore(finalScore);
                    if (isNewHigh)
                    {
                        gameOverHighScoreText.text = $"NEW HIGH SCORE!";
                        gameOverHighScoreText.color = Color.yellow;
                    }
                    else
                    {
                        gameOverHighScoreText.text = $"Best: {scoreManager.ScoreData.AllTimeHighScore}";
                        gameOverHighScoreText.color = Color.white;
                    }
                }
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
            
            if (levelCompleteScoreText != null && scoreManager != null)
            {
                levelCompleteScoreText.text = $"Score: {scoreManager.GetFinalScore()}";
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

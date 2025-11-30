using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private float scorePerSecondSharpening = 10f;
    [SerializeField] private float comboMultiplierIncrement = 0.1f;
    [SerializeField] private float maxComboMultiplier = 5f;
    [SerializeField] private float comboDecayTime = 1f;
    
    [Header("Score Data")]
    [SerializeField] private ScoreData scoreData;
    
    public event Action<int> OnScoreChanged;
    public event Action<float> OnComboChanged;
    public event Action<int> OnNewHighScore;
    
    private int currentScore;
    private float accumulatedScore;
    private float comboMultiplier = 1f;
    private float timeSinceLastScore;
    private bool isSharpening;
    private float sharpeningTime;
    
    public int CurrentScore => currentScore;
    public float ComboMultiplier => comboMultiplier;
    public ScoreData ScoreData => scoreData;
    
    private void Awake()
    {
        if (scoreData != null)
        {
            scoreData.LoadFromPlayerPrefs();
        }
    }
    
    private void Update()
    {
        if (isSharpening)
        {
            sharpeningTime += Time.deltaTime;
            
            float scoreThisFrame = scorePerSecondSharpening * comboMultiplier * Time.deltaTime;
            accumulatedScore += scoreThisFrame;
            
            int scoreToAdd = Mathf.FloorToInt(accumulatedScore);
            if (scoreToAdd > 0)
            {
                accumulatedScore -= scoreToAdd;
                AddScore(scoreToAdd);
            }
            
            timeSinceLastScore = 0f;
            
            if (comboMultiplier < maxComboMultiplier)
            {
                comboMultiplier += comboMultiplierIncrement * Time.deltaTime;
                comboMultiplier = Mathf.Min(comboMultiplier, maxComboMultiplier);
                OnComboChanged?.Invoke(comboMultiplier);
            }
        }
        else
        {
            timeSinceLastScore += Time.deltaTime;
            
            if (timeSinceLastScore >= comboDecayTime && comboMultiplier > 1f)
            {
                comboMultiplier = Mathf.Max(1f, comboMultiplier - comboMultiplierIncrement * Time.deltaTime * 2f);
                OnComboChanged?.Invoke(comboMultiplier);
            }
        }
    }
    
    public void StartSharpening()
    {
        isSharpening = true;
        sharpeningTime = 0f;
    }
    
    public void StopSharpening()
    {
        isSharpening = false;
    }
    
    public void AddScore(int amount)
    {
        if (amount <= 0)
            return;
        
        currentScore += amount;
        OnScoreChanged?.Invoke(currentScore);
        
        if (scoreData != null)
        {
            bool wasNewHigh = scoreData.IsNewHighScore(currentScore);
            scoreData.UpdateScore(currentScore);
            
            if (wasNewHigh && scoreData.IsNewHighScore(currentScore))
            {
                OnNewHighScore?.Invoke(currentScore);
            }
        }
    }
    
    public void ResetScore()
    {
        currentScore = 0;
        accumulatedScore = 0f;
        comboMultiplier = 1f;
        sharpeningTime = 0f;
        timeSinceLastScore = 0f;
        isSharpening = false;
        
        OnScoreChanged?.Invoke(currentScore);
        OnComboChanged?.Invoke(comboMultiplier);
    }
    
    public int GetFinalScore()
    {
        return currentScore;
    }
}

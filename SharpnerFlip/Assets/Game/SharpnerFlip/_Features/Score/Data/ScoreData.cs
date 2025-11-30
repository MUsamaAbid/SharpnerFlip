using UnityEngine;

[CreateAssetMenu(fileName = "ScoreData", menuName = "SharpnerFlip/Score Data", order = 1)]
public class ScoreData : ScriptableObject
{
    [Header("High Scores")]
    [SerializeField] private int allTimeHighScore;
    [SerializeField] private int sessionHighScore;
    
    public int AllTimeHighScore => allTimeHighScore;
    public int SessionHighScore => sessionHighScore;
    
    public void UpdateScore(int newScore)
    {
        if (newScore > sessionHighScore)
        {
            sessionHighScore = newScore;
        }
        
        if (newScore > allTimeHighScore)
        {
            allTimeHighScore = newScore;
            SaveToPlayerPrefs();
        }
    }
    
    public void LoadFromPlayerPrefs()
    {
        allTimeHighScore = PlayerPrefs.GetInt("HighScore", 0);
        sessionHighScore = 0;
    }
    
    public void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetInt("HighScore", allTimeHighScore);
        PlayerPrefs.Save();
    }
    
    public void ResetAllTimeHighScore()
    {
        allTimeHighScore = 0;
        SaveToPlayerPrefs();
    }
    
    public void ResetSessionScore()
    {
        sessionHighScore = 0;
    }
    
    public bool IsNewHighScore(int score)
    {
        return score > allTimeHighScore;
    }
    
    public bool IsNewSessionBest(int score)
    {
        return score > sessionHighScore;
    }
}

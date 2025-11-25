using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    public int levelNumber = 1;
    public string levelName = "Level 1";
    
    [Header("Collectibles to Spawn")]
    public List<CollectibleType> collectibleTypes = new List<CollectibleType>();
    public int totalCollectibles = 10;
    public bool randomizeOrder = true;
    
    [Header("Win Condition")]
    public int collectiblesRequiredToWin = 8;
}

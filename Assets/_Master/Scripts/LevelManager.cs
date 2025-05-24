using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelManager", menuName = "Scriptable Objects/LevelManager")]
public class LevelManager : ScriptableObject
{
    public List<Level> levels;
    public Level GetLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Count)
        {
            Debug.LogWarning("Level index out of range!");
            return levels[levels.Count-1];
        }
        return levels[levelIndex];
    }
    public int GetTotalLevelCount()
    {
        return levels.Count;
    }

}

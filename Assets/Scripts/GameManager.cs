using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Level")]
    public int level;
    public int width = 9;
    public int height = 12;
    public bool[,] walkableMap;
    public bool[,] trapSpaces;
    public bool[,] breakableSpaces;
    public bool[,] enemySpaces;
    public bool[,] bossSpaces;

    private string saveFilePath;

    public void Awake()
    {

        saveFilePath = Path.Combine(Application.persistentDataPath, "gameData.json");

        LoadGameData();
        Init();
    }

    private void Init()
    {
        trapSpaces = new bool[width, height];
        breakableSpaces = new bool[width, height];
        walkableMap = new bool[width, height];
        enemySpaces = new bool[width, height];
        bossSpaces = new bool[width, height];
    }
    public void SaveGameData()
    {
        GameData data = new GameData
        {
            level = this.level
        };

        string json = JsonUtility.ToJson(data, true); 
        File.WriteAllText(saveFilePath, json); 
        Debug.Log($"Game data saved to {saveFilePath}");
    }

    public void LoadGameData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath); 
            GameData data = JsonUtility.FromJson<GameData>(json); 
            this.level = data.level;
            Debug.Log($"Game data loaded from {saveFilePath}");
        }
        else
        {
            level = 1;
            SaveGameData(); 
        }
    }
}

[System.Serializable]
public class GameData
{
    public int level;
}


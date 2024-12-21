using System.Collections;
using System.Collections.Generic;
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
    public bool[,] bossSpaces;
    public void Awake()
    {
        if (PlayerPrefs.HasKey("Level"))
        {
            level = PlayerPrefs.GetInt("Level",1);
        }
        else
        {
            level = 1;
            PlayerPrefs.SetInt("Level", 1);
            PlayerPrefs.Save();
        }
        Init();
    }

    private void Init()
    {
        trapSpaces = new bool[width, height];
        breakableSpaces = new bool[width, height];
        walkableMap = new bool[width, height];
        bossSpaces = new bool[width, height];
    }
}

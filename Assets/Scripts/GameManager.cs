using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int level;
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
    }
}

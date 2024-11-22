using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObjects/Map", order = 1)]
public class MapData : ScriptableObject
{
    [Tooltip("1<x<9 && y<12")]
    //x :width
    //y: height
    public TileType[] tileTypes;

    private void Reset()
    {
        int width = 9;
        int height = 12;
        tileTypes = new TileType[width * height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                int index = i * height + j;
                tileTypes[index] = new TileType();
                tileTypes[index].x = i;
                tileTypes[index].y = j;
                tileTypes[index].tileKind = TileKind.Wall;
                Debug.Log(index);
            }
        }
    }

}

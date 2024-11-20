using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public enum TileKind
{
    Wall,
    Normal
}
[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}
public class CreateMap : MonoBehaviour
{
    [SerializeField] int width=9;
    [SerializeField] int height=12;
    public TileType[] layouts;
    public GameObject tileBackGround;
    public GameObject wallPrefabs;
    private GameObject[,] maps;
    public List<ColliderBackGround> colliders;
    public bool[,] wallSpace;
    public LevelObject levelObjects;

    private void Start()
    {
        layouts = levelObjects.mapData[GameManager.Instance.level - 1].tileTypes;
        maps=new GameObject[width,height];
        wallSpace = new bool[width, height];
        CheckWallTiles();
        InitMap();
    }
    private void InitMap()
    {
        for(int i=1;i<width;i++)
        {
            for(int j=0;j<height;j++)
            {
                if (wallSpace[i, j])
                {
                    GameObject wallTmp = Instantiate(wallPrefabs, this.transform);
                    wallTmp.transform.position = new Vector3(i, j, 89f);
                    wallTmp.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                    wallTmp.name= "(wall: " + i + "," + j + ")";
                }
                else
                {
                    GameObject tile = Instantiate(tileBackGround, this.transform);
                    tile.transform.position = new Vector3(i, j, 90.5f);
                    tile.transform.rotation = Quaternion.identity;
                    tile.name = "(" + i + "," + j + ")";
                    maps[i, j] = tile;
                    colliders.Add(tile.GetComponent<ColliderBackGround>());
                }
            }    
        }
    }
    public void CheckWallTiles()//Start
    {
        for(int i=0;i<layouts.Length;i++)
        {
            if (layouts[i].tileKind == TileKind.Wall)
            {
                wallSpace[layouts[i].x, layouts[i].y] = true;
            }
        }
  
    }
    public bool CheckCount()
    {
        int num = 0;
        for(int i = 0; i < colliders.Count; i++)
        {
            num+= colliders[i].isActive;
        }
        if (num == 0) { return true; }
        return false;
    }

      
}

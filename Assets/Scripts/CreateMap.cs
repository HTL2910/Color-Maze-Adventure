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
   
    [SerializeField] int width = 9;
    [SerializeField] int height = 12;
    public GameObject tileBackGround;
    public GameObject wallPrefabs;
    public LevelObject levelObjects;

    private GameObject[,] maps;
    private List<ColliderBackGround> colliders;
    protected int level;
    int index;
    private void Awake()
    {
        colliders = new List<ColliderBackGround>();
    }

    private void Start()
    {

        level = PlayerPrefs.GetInt("Level", 1);
        if (levelObjects.list_Matrix.Count < level)
        {
            int tmp=(int) Random.Range(0,levelObjects.list_Matrix.Count);
            index = tmp;
        }
        else
        {
            index = level - 1;
        }
        maps = new GameObject[width, height];
        InitMap();
    }

    private void InitMap()
    {
        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < height; j++)
            {
                if (levelObjects.list_Matrix[index].GetValue(i,j) == false) 
                {
                    GameObject wallTmp = Instantiate(wallPrefabs, this.transform);
                    wallTmp.transform.position = new Vector3(i, j, 89f);
                    wallTmp.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                    wallTmp.name = $"(wall: {i},{j})";
                }
                else 
                {
                    GameObject tile = Instantiate(tileBackGround, this.transform);
                    tile.transform.position = new Vector3(i, j, 90.5f);
                    tile.transform.rotation = Quaternion.identity;
                    tile.name = $"({i},{j})";
                    maps[i, j] = tile;

                  
                    ColliderBackGround collider = tile.GetComponent<ColliderBackGround>();
                    if (collider != null)
                    {
                        colliders.Add(collider);
                    }
                    else
                    {
                        Debug.LogWarning($"Tile tại ({i},{j}) không có ColliderBackGround.");
                    }
                }
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

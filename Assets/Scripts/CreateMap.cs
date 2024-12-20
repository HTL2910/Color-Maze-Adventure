using System;
using System.Collections.Generic;
using UnityEngine;

public enum TileTrap
{
    Trap, 
    Breakable,
    Boss
}
[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileTrap tileKind;
}
public class CreateMap : MonoBehaviour
{
   
    [SerializeField] int width = 9;
    [SerializeField] int height = 12;
    public GameObject tileBackGround;
    public GameObject wallPrefabs;
    public GameObject trapPrefabs;
    public GameObject ball;
    public LevelObject levelObjects;
    private GameObject[] walls;
    private GameObject[,] maps;
    public TileType[] trapInMap;
    private bool[,] trapSpaces;
    private List<ColliderBackGround> colliders;
    protected int level;
    int index;
    [SerializeField] Material wallMaterial;
    [SerializeField] Material ballMaterial;
    [SerializeField] Color colorBackground;
    [SerializeField] Color newColorBackground;

    private BoolMatrix boolMatrix;
    private void Awake()
    {
        colliders = new List<ColliderBackGround>();
    }

    private void Start()
    {
        walls = GameObject.FindGameObjectsWithTag("Wall");
        trapSpaces=new bool[width, height];
        
        InitGamePlay();
        SetMaterial();
        GenTrap();
        InitMap();
    }
    private void GenTrap()
    {
        for (int i = 0; i < trapInMap.Length; i++)
        {
            if (trapInMap[i].tileKind == TileTrap.Trap)
            {
                trapSpaces[trapInMap[i].x, trapInMap[i].y] = true;
            }
        }
    }
    private void SetMaterial()
    {
        Material(ball, ballMaterial);
        foreach (GameObject wall in walls)
        {
            Material(wall, wallMaterial);
        }
    }

    private void InitGamePlay()
    {
        level = PlayerPrefs.GetInt("Level", 1);
        if (levelObjects.list_Matrix.Count < level)
        {
            int tmp = (int)UnityEngine.Random.Range(0, levelObjects.list_Matrix.Count);
            index = tmp;
        }
        else
        {
            index = level - 1;
        }
        boolMatrix = levelObjects.list_Matrix[index];
        ballMaterial = boolMatrix.ballMaterial;
        wallMaterial = boolMatrix.wallMaterial;
        trapInMap=boolMatrix.trapInMap;
        colorBackground = boolMatrix.colorBackground;
        newColorBackground = boolMatrix.newColorBackground;
        maps = new GameObject[width, height];
    }

    private void Material(GameObject obj,Material mat)
    {
        MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
        Material[] mats = meshRenderer.materials;
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i] = mat;
        }
        meshRenderer.materials = mats;
    }

    private void InitMap()
    {
        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < height; j++)
            {
                if (boolMatrix.GetValue(i,j) == false) 
                {
                    GameObject wallTmp = Instantiate(wallPrefabs, this.transform);
                    Material(wallTmp,wallMaterial);
                    wallTmp.transform.position = new Vector3(i, j, 89f);
                    wallTmp.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                    wallTmp.name = $"(wall: {i},{j})";
                }
                else 
                {
                    if (trapSpaces[i,j]==true)
                    {
                        GameObject trap = Instantiate(trapPrefabs, this.transform);
                        trap.transform.position = new Vector3(i, j, 91f);
                        trap.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
                        trap.name = $"Trap: ({i},{j})";
                    }
                    else
                    {
                        GameObject tile = Instantiate(tileBackGround, this.transform);
                        tile.GetComponent<SpriteRenderer>().color = colorBackground;
                        tile.GetComponent<ColliderBackGround>().newColor = newColorBackground;
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

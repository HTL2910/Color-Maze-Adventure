using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
    int width;
    int height; 
    [Header("GameObject")]
    public GameObject tileBackGround;
    public GameObject wallPrefabs;
    public GameObject trapPrefabs;
    public GameObject breakablePrefabs;
    public GameObject bossPrefabs;
    public GameObject ball;
    public LevelObject levelObjects;
    private GameObject[] walls;
    private GameObject[,] maps;
    public TileType[] trapInMap;

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
                GameManager.Instance.trapSpaces[trapInMap[i].x, trapInMap[i].y] = true;
            }
            if (trapInMap[i].tileKind == TileTrap.Breakable)
            {
                GameManager.Instance.breakableSpaces[trapInMap[i].x, trapInMap[i].y] = true;
            }
            if (trapInMap[i].tileKind == TileTrap.Boss)
            {
                GameManager.Instance.bossSpaces[trapInMap[i].x, trapInMap[i].y] = true;
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
        width = GameManager.Instance.width;
        height = GameManager.Instance.height;
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
                    if (GameManager.Instance.breakableSpaces[i, j] == true)
                    {
                        GameObject breakObj = Instantiate(breakablePrefabs, this.transform);
                        breakObj.transform.position = new Vector3(i, j, 89.5f);
                        breakObj.name = $"breakObj: ({i},{j})";
                    }
                    if (GameManager.Instance.bossSpaces[i,j] == true)
                    {
                        GameObject bossObj = Instantiate(bossPrefabs, this.transform);
                        bossObj.transform.position = new Vector3(i, j, 89.5f);
                        bossObj.name = $"AI: ({i},{j})";
                    }
                    if (GameManager.Instance.trapSpaces[i,j]==true)
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
                    GameManager.Instance.walkableMap[i, j] = true;
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

using System;
using System.Collections.Generic;
using UnityEngine;


public class CreateMap : MonoBehaviour
{
   
    [SerializeField] int width = 9;
    [SerializeField] int height = 12;
    public GameObject tileBackGround;
    public GameObject wallPrefabs;
    public GameObject ball;
    public LevelObject levelObjects;
    private GameObject[] walls;
    private GameObject[,] maps;
    private List<ColliderBackGround> colliders;
    protected int level;
    int index;
    [SerializeField] Material wallMaterial;
    [SerializeField] Material ballMaterial;
    [SerializeField] Color colorBackground;
    [SerializeField] Color newColorBackground;
    private void Awake()
    {
        colliders = new List<ColliderBackGround>();
    }

    private void Start()
    {
        walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach(GameObject wall in walls)
        {
            Material(wall, wallMaterial);
        }
        level = PlayerPrefs.GetInt("Level", 1);
        if (levelObjects.list_Matrix.Count < level)
        {
            int tmp=(int) UnityEngine.Random.Range(0,levelObjects.list_Matrix.Count);
            index = tmp;
        }
        else
        {
            index = level - 1;
        }
        maps = new GameObject[width, height];
        Material(ball, ballMaterial);
        InitMap();
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
                if (levelObjects.list_Matrix[index].GetValue(i,j) == false) 
                {
                    GameObject wallTmp = Instantiate(wallPrefabs, this.transform);
                    Material(wallTmp,wallMaterial);
                    wallTmp.transform.position = new Vector3(i, j, 89f);
                    wallTmp.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                    wallTmp.name = $"(wall: {i},{j})";
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMap : MonoBehaviour
{
    [SerializeField] int width=10;
    [SerializeField] int height=10;

    public GameObject tileBackGround;
    public GameObject[,] maps;
    
   
    private void Start()
    {
        maps=new GameObject[width,height];
        
        InitMap();
    }
    private void InitMap()
    {
        for(int i=1;i<width;i++)
        {
            for(int j=0;j<height;j++)
            {
                GameObject tile = Instantiate(tileBackGround, this.transform);
                tile.transform.position = new Vector3(i, j, 90.5f);
                tile.transform.rotation=Quaternion.identity;
                tile.name= "(" + i + "," + j + ")";
                maps[i,j]= tile;
            }    
        }
        

    }
}

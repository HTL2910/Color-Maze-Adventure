using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GroupItem : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Image image;
    [SerializeField] private Transform content;
    [SerializeField] List<GameObject> itemsList;
    [SerializeField] private ImageResources imageResources;
    [SerializeField] private Image maxImage;
    [SerializeField]private List<ImageData> listImageData;
    private int totalTypes = 6;
    public int currentType=1;
    public bool isRandomType=false;
    void Start()
    {
        Invoke(nameof(GenerateItem), 0.5f);
    }

    private void GenerateItem()
    {
        listImageData = Generate25Images(imageResources.allImages);
        for (int i = 0; i < listImageData.Count; i++)
        {
            GameObject item = Instantiate(itemPrefab, content);
            Item itemScript = item.GetComponent<Item>();
            itemScript.index = listImageData[i].rank;
            itemScript.image.texture = listImageData[i].sprite.texture;
            itemsList.Add(item);
        }
    }

    public List<ImageData> Generate25Images(List<ImageData> allImages)
    {
        List<ImageData> selected = new List<ImageData>();
        HashSet<ImageData> used = new HashSet<ImageData>();
        System.Random rng = new System.Random();

        selected.Clear();
        used.Clear();
       
        
        for (int i = 0; i < 6; i++)
        {
            currentType= isRandomType? rng.Next(1, totalTypes + 1):currentType;
            int rankMin = 1 + 4 * i;  
            int rankMax = rankMin + 3; 
            Debug.Log($"rankMin: {rankMin}, rankMax: {rankMax}");
            var group = allImages
                .Where(img => img.typeIndex == currentType && 
                    img.rank >= rankMin && img.rank <= rankMax && 
                    !used.Contains(img))
                    .OrderBy(x => Random.value)
                    .Take(4)
                    .ToList();


            selected.AddRange(group);
            foreach (var img in group) used.Add(img);
            if(i==5)
            {
                maxImage.sprite=allImages.Where(img=>img.typeIndex==currentType && img.rank==25).First().sprite;
            }
            
        }

        return selected.OrderBy(x => UnityEngine.Random.value).ToList();
    }
    
    public bool CheckOrder(List<ImageData> currentOrder)
    {
        for (int i = 0; i < currentOrder.Count; i++)
        {
            if (currentOrder[i].rank != i + 1)
                return false;
        }
        return true;
    }

    public void OnOKButtonClick()
    {
        listImageData.Clear();
        for (int i = 0; i < content.childCount; i++)
        {
            Transform child = content.GetChild(i);
            Item itemScript = child.GetComponent<Item>();
            RawImage rawImage = child.GetComponentInChildren<RawImage>();
            if (itemScript != null && rawImage != null)
            {
                ImageData imageData = new ImageData
                {
                    rank = itemScript.index,
                    typeIndex = currentType,
                    sprite = Sprite.Create(rawImage.texture as Texture2D, new Rect(0, 0, rawImage.texture.width, rawImage.texture.height), new Vector2(0.5f, 0.5f))
                };
                listImageData.Add(imageData);
            }
        }

        if (CheckOrder(listImageData))
        {
            Debug.Log("Đúng");
        }
        else
        {
            Debug.Log("Sai");
        }
    }
}

using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
public class GroupItem : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private UnityEngine.UI.Image image;
    [SerializeField] private Transform content;
    [SerializeField] private List<GameObject> itemsList;
    [SerializeField] private ImageResources imageResources;
    public List<ImageData> listImageData;
    private int countItem = 24;

    void Start()
    {
        Invoke(nameof(GenerateItem), 0.5f);
        // listImageData = imageResources.GenerateRandom25();
    }
    private void GenerateItem()
    {
        for (int i = 0; i < countItem; i++)
        {
            GameObject item = Instantiate(itemPrefab, content);
            Item itemScript = item.GetComponent<Item>();
            itemScript.index = i;
            itemScript.image.texture = imageResources.allImages[i].sprite.texture;
            itemsList.Add(item);
        }
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
}

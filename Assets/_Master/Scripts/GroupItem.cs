using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GroupItem : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform content;
    [SerializeField] private Image maxImage;
    [SerializeField] private ImageResources imageResources;
    private List<ImageData> listImageData = new List<ImageData>();
    private List<GameObject> itemsList = new List<GameObject>();
    void Start()
    {
        Invoke(nameof(GenerateItem), 0.5f);
    }

    private void GenerateItem()
    {
        // Lấy level hiện tại
        Debug.Log(GameManager.Instance.currentLevelIndex);
        Level currentLevel = GameManager.Instance.levelManager.GetLevel(GameManager.Instance.currentLevelIndex);
        UIManager.instance.SetFlagImage(currentLevel.background);
        listImageData = currentLevel.levelImages;

        
            var max = listImageData[listImageData.Count-1];
            if (max != null)
            {
                maxImage.sprite = max.sprite;
            }
            var shuffledImages = listImageData.OrderBy(x => Random.value).ToList();
            for (int i = 0; i < shuffledImages.Count; i++)
            {
                if(shuffledImages[i]!=max)
                {
                    GameObject item = Instantiate(itemPrefab, content);
                    Item itemScript = item.GetComponent<Item>();
                    itemScript.index = shuffledImages[i].rank;
                    itemScript.image.texture = shuffledImages[i].sprite.texture;
                    itemsList.Add(item);
                }
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

    public void OnOKButtonClick()
    {
        List<ImageData> currentList = new List<ImageData>();

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
                    typeIndex = GameManager.Instance.CurrentLevel.levelImages[itemScript.index - 1].typeIndex,
                    sprite = Sprite.Create(rawImage.texture as Texture2D, new Rect(0, 0, rawImage.texture.width, rawImage.texture.height), new Vector2(0.5f, 0.5f))
                };
                currentList.Add(imageData);
            }
        }

        if (CheckOrder(currentList))
        {
            UIManager.instance.WinGame();
        }
        else
        {
            UIManager.instance.LoseGame();
        }
    }
}

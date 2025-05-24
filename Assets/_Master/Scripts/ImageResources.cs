using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class ImageData
{
    public Sprite sprite;
    public int typeIndex;
    public int rank;
}
public class ImageResources : MonoBehaviour
{
    public List<ImageData> allImages;

    void Reset()
    {
        allImages = LoadAllImages();
    }

    public List<ImageData> LoadAllImages()//hàm lấy tất cả ảnh từ folder
    {
        List<ImageData> allImages = new List<ImageData>();

        for (int folderIndex = 1; folderIndex <= 6; folderIndex++)
        {
            string folderPath = $"Type{folderIndex}";
                
            Sprite[] sprites = Resources.LoadAll<Sprite>(folderPath);

            for (int i = 0; i < sprites.Length; i++)
            {
                allImages.Add(new ImageData
                {
                    sprite = sprites[i],
                    typeIndex = folderIndex,
                    rank = i + 1 // Ảnh thứ i tương ứng cấp i+1
                });
            }
        }

        return allImages;
    }
}

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using System.Linq;

[System.Serializable]
public class ImageData
{
    public Sprite sprite;
    public int rank;
}
public class ImageResources : MonoBehaviour
{
    public List<ImageData> allImages;

    void Start()
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
                    rank = i + 1 // Ảnh thứ i tương ứng cấp i+1
                });
            }
        }

        return allImages;
    }
    public List<ImageData> GenerateRandom25()// hàm lấy 25 ảnh ngẫu nhiên
    {
        List<ImageData> allImages = LoadAllImages();
        List<ImageData> selected = new List<ImageData>();

        for (int rank = 1; rank <= 25; rank++)
        {
            var candidates = allImages.Where(img => img.rank == rank).ToList();
            if (candidates.Count > 0)
            {
                var randomImg = candidates[UnityEngine.Random.Range(0, candidates.Count)];
                selected.Add(randomImg);
            }
        }

        // Shuffle danh sách
        return selected.OrderBy(x => UnityEngine.Random.value).ToList();
    }
}

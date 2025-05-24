using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/Level")]
public class Level : ScriptableObject
{
    public Sprite background;
    public float timeLimit;

    [Header("Index range for copying")]
    public int startIndex;
    public int endIndex;

    [Header("All images for this level")]
    public List<ImageData> allImages = new List<ImageData>();

    [Header("Level-specific images")]
    public List<ImageData> levelImages = new List<ImageData>();
    private void OnEnable()
    {
    #if UNITY_EDITOR
        if (allImages == null || allImages.Count == 0)
        {
            allImages = LoadAllImages();
            CopyFromAllImages();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    #endif
    }

    public void CopyFromAllImages()
    {
        levelImages.Clear();

        if (allImages == null || allImages.Count == 0)
        {
            Debug.LogWarning("⚠️ allImages is empty.");
            return;
        }

        int start = Mathf.Clamp(startIndex, 0, allImages.Count - 1);
        int end = Mathf.Clamp(endIndex, 0, allImages.Count - 1);

        if (start > end)
        {
            Debug.LogWarning("⚠️ startIndex must be <= endIndex.");
            return;
        }

        for (int i = start; i <= end; i++)
        {
            levelImages.Add(allImages[i]);
        }
       
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

    public void PasteToAllImages()
    {
        allImages.Clear();
        allImages.AddRange(levelImages);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    /// <summary>
    /// Randomize levelImages while maintaining the same count and ensuring no duplicates
    /// </summary>
    public void RandomizeLevelImages()
    {
        if (levelImages == null || levelImages.Count == 0)
        {
            Debug.LogWarning("⚠️ levelImages is empty. Copy images first!");
            return;
        }

        // Create a copy of the current level images
        List<ImageData> tempList = new List<ImageData>(levelImages);
        
        // Clear and refill with randomized order
        levelImages.Clear();
        
        while (tempList.Count > 0)
        {
            // Get random index
            int randomIndex = Random.Range(0, tempList.Count);
            
            // Add the randomly selected item and remove it from temp list
            levelImages.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}

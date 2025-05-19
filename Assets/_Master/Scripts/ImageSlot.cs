using UnityEngine;
using UnityEngine.UI;
public class ImageSlot : MonoBehaviour
{
    public Image imageComponent;
    public int rank;

    public void SetData(Sprite sprite, int newRank)
    {
        imageComponent.sprite = sprite;
        rank = newRank;
    }

    public Sprite GetSprite()
    {
        return imageComponent.sprite;
    }

    public int GetRank()
    {
        return rank;
    }
}

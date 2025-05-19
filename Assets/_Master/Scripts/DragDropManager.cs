using UnityEngine;

public class DragDropManager : MonoBehaviour
{
    public static DragDropManager Instance;

    private ImageSlot draggedSlot;

    private void Awake()
    {
        Instance = this;
    }

    public void OnBeginDrag(ImageSlot slot)
    {
        draggedSlot = slot;
    }

    public void OnDrop(ImageSlot targetSlot)
    {
        if (draggedSlot == null || targetSlot == null || draggedSlot == targetSlot)
            return;

        // Hoán đổi sprite và rank
        Sprite tempSprite = draggedSlot.GetSprite();
        int tempRank = draggedSlot.GetRank();

        draggedSlot.SetData(targetSlot.GetSprite(), targetSlot.GetRank());
        targetSlot.SetData(tempSprite, tempRank);

        draggedSlot = null;
    }
}

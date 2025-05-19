using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Transform originalParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.position;
        originalParent = transform.parent;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position += (Vector3)eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.position = originalPosition;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var droppedObject = eventData.pointerDrag;
        if (droppedObject == null || droppedObject == gameObject) return;

        Item thisItem = GetComponent<Item>();
        Item otherItem = droppedObject.GetComponent<Item>();

        if (thisItem != null && otherItem != null)
        {
            // Hoán đổi texture
            Texture tempTexture = thisItem.image.texture;
            thisItem.image.texture = otherItem.image.texture;
            otherItem.image.texture = tempTexture;

            // Hoán đổi index
            int tempIndex = thisItem.index;
            thisItem.index = otherItem.index;
            otherItem.index = tempIndex;
        }
    }
}

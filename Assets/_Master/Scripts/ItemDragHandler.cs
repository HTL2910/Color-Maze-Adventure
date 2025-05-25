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
        AudioManager.Instance.PlayDragAndDropSound();
        originalPosition = rectTransform.position;
        originalParent = transform.parent;
        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out position);
        
        rectTransform.position = canvas.transform.TransformPoint(position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent);
        rectTransform.position = originalPosition;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
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

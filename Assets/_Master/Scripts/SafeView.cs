using UnityEngine;

public class SafeView : MonoBehaviour
{
    [SerializeField] private GameObject safeView;
    [SerializeField] private float safeAreaWidth = 0.9f;  // 90% of screen width
    [SerializeField] private float safeAreaHeight = 0.9f; // 90% of screen height
    
    private Rect safeArea;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        InitializeSafeArea();
    }

    private void InitializeSafeArea()
    {
        // Create safe view area
        safeView.transform.SetParent(transform);
        safeView.transform.localPosition = Vector3.zero;
        safeView.transform.localRotation = Quaternion.identity;
        safeView.transform.localScale = Vector3.one;

        // Calculate safe area in screen coordinates
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        
        float safeWidth = screenWidth * safeAreaWidth;
        float safeHeight = screenHeight * safeAreaHeight;
        
        float xOffset = (screenWidth - safeWidth) / 2;
        float yOffset = (screenHeight - safeHeight) / 2;
        
        safeArea = new Rect(xOffset, yOffset, safeWidth, safeHeight);
    }

    public bool IsWithinSafeArea(Vector3 worldPosition)
    {
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(worldPosition);
        return safeArea.Contains(screenPoint);
    }

    public Vector3 ClampToSafeArea(Vector3 worldPosition)
    {
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(worldPosition);
        
        // Clamp the screen point to the safe area
        screenPoint.x = Mathf.Clamp(screenPoint.x, safeArea.xMin, safeArea.xMax);
        screenPoint.y = Mathf.Clamp(screenPoint.y, safeArea.yMin, safeArea.yMax);
        
        // Convert back to world position
        return mainCamera.ScreenToWorldPoint(screenPoint);
    }

    // Call this method to ensure an object stays within the safe area
    public void KeepObjectInSafeArea(Transform objectTransform)
    {
        if (!IsWithinSafeArea(objectTransform.position))
        {
            objectTransform.position = ClampToSafeArea(objectTransform.position);
        }
    }
}

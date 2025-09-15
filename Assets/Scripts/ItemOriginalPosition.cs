using UnityEngine;

public class ItemOriginalPosition : MonoBehaviour
{
    [Header("元の位置設定")]
    [SerializeField] private Vector2 originalPosition;
    [SerializeField] private Transform originalParent;
    [SerializeField] private bool savePositionOnStart = true;
    
    void Start()
    {
        if (savePositionOnStart)
        {
            SaveOriginalPosition();
        }
    }
    
    public void SaveOriginalPosition()
    {
        RectTransform rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            originalPosition = rect.anchoredPosition;
            originalParent = transform.parent;
            
            Debug.Log($"元の位置を保存: {gameObject.name} at {originalPosition}");
        }
    }
    
    public void ReturnToOriginalPosition()
    {
        if (originalParent != null)
        {
            transform.SetParent(originalParent);
            GetComponent<RectTransform>().anchoredPosition = originalPosition;
            
            Debug.Log($"元の位置に復帰: {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"元の位置情報がありません: {gameObject.name}");
        }
    }
    
    public Vector2 GetOriginalPosition()
    {
        return originalPosition;
    }
    
    public Transform GetOriginalParent()
    {
        return originalParent;
    }
}

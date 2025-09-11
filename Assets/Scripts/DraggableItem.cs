using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("スナップ設定")]
    [SerializeField] private float snapDistance = 1f;
    [Header("デバッグ")]
    [SerializeField] private bool showDebugInfo = false;
    
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform originalParent;
    private DropSlot nearestSlot;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        
        Debug.Log($"DraggableItem初期化: {gameObject.name}");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"ドラッグ開始: {gameObject.name}");
        
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        UpdateNearestSlot();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"ドラッグ終了: {gameObject.name}");
        
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        bool dropSuccessful = false;
        
        // 1. 直接ドロップを試行
        DropSlot directSlot = null;
        if (eventData.pointerEnter != null)
        {
            directSlot = eventData.pointerEnter.GetComponent<DropSlot>();
        }
        
        if (directSlot != null && !directSlot.HasItem())
        {
            Debug.Log("直接ドロップ成功!");
            directSlot.PlaceItem(gameObject);
            dropSuccessful = true;
        }
        // 2. スナップを試行
        else if (nearestSlot != null && !nearestSlot.HasItem())
        {
            Debug.Log($"スナップドロップ成功: {nearestSlot.name}");
            nearestSlot.PlaceItem(gameObject);
            dropSuccessful = true;
        }
        
        // 3. 失敗した場合は元の位置に戻す
        if (!dropSuccessful)
        {
            Debug.Log("ドロップ失敗 - 元の位置に戻します");
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = originalPosition;
        }
        
        ClearAllHighlights();
        nearestSlot = null;
    }
    
    private void UpdateNearestSlot()
    {
        DropSlot[] allSlots = FindObjectsByType<DropSlot>(FindObjectsSortMode.None);
        DropSlot closestSlot = null;
        float closestDistance = float.MaxValue;
        
        // 前回のハイライトをクリア
        if (nearestSlot != null)
        {
            nearestSlot.SetSnapHighlight(false);
        }
        
        foreach (DropSlot slot in allSlots)
        {
            if (slot.HasItem()) continue;
            
            // 重要：両方のオブジェクトが同じ座標系で距離を計算
            float distance = GetUIDistance(rectTransform, slot.GetComponent<RectTransform>());
            
            if (showDebugInfo)
            {
                Debug.Log($"スロット: {slot.name}, 距離: {distance:F1}, 判定距離: {snapDistance}");
            }
                
            if (distance < closestDistance && distance <= snapDistance)
            {
                closestDistance = distance;
                closestSlot = slot;
            }
        }
        
        nearestSlot = closestSlot;
        if (nearestSlot != null)
        {
            nearestSlot.SetSnapHighlight(true);
            if (showDebugInfo) Debug.Log($"スナップハイライト: {nearestSlot.name} (距離: {closestDistance:F1})");
        }
    }
    
    // UI要素間の正確な距離を計算
    private float GetUIDistance(RectTransform rect1, RectTransform rect2)
    {
        // 両方のRectTransformのワールド座標を取得
        Vector3 pos1 = rect1.position;
        Vector3 pos2 = rect2.position;
        
        // ワールド座標での距離を計算してUI座標に変換
        float worldDistance = Vector3.Distance(pos1, pos2);
        
        // Canvas の scale factor を考慮
        return worldDistance / canvas.scaleFactor;
    }
    
    private void ClearAllHighlights()
    {
        DropSlot[] allSlots = FindObjectsByType<DropSlot>(FindObjectsSortMode.None);
        foreach (DropSlot slot in allSlots)
        {
            slot.SetSnapHighlight(false);
        }
    }
    
    // Scene view でスナップ範囲を表示
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, snapDistance * canvas.scaleFactor);
        }
    }
}

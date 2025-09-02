using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("スナップ設定")]
    [SerializeField] private float snapDistance = 80f; // スナップする距離
    
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
        
        // ドラッグ中に最も近いスロットを検索
        UpdateNearestSlot();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"ドラッグ終了: {gameObject.name}");
        Debug.Log($"pointerEnter: {eventData.pointerEnter?.name}");
        
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        bool dropSuccessful = false;
        
        // 1. まず直接ドロップを試行
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
        // 2. 直接ドロップできなかった場合、スナップを試行
        else if (nearestSlot != null && !nearestSlot.HasItem())
        {
            Debug.Log($"スナップドロップ成功: {nearestSlot.name}");
            nearestSlot.PlaceItem(gameObject);
            dropSuccessful = true;
        }
        
        // 3. どちらも失敗した場合は元の位置に戻す
        if (!dropSuccessful)
        {
            Debug.Log("ドロップ失敗 - 元の位置に戻します");
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = originalPosition;
        }
        
        // ハイライトをクリア
        ClearAllHighlights();
        nearestSlot = null;
    }
    
    private void UpdateNearestSlot()
    {
        // 全てのDropSlotを検索
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
            // 既にアイテムが配置されているスロットはスキップ
            if (slot.HasItem()) continue;
            
            // 距離を計算
            float distance = Vector2.Distance(
                rectTransform.anchoredPosition, 
                slot.GetComponent<RectTransform>().anchoredPosition
            );
                
            if (distance < closestDistance && distance <= snapDistance)
            {
                closestDistance = distance;
                closestSlot = slot;
            }
        }
        
        // 最も近いスロットをハイライト
        nearestSlot = closestSlot;
        if (nearestSlot != null)
        {
            nearestSlot.SetSnapHighlight(true);
            Debug.Log($"スナップ範囲: {nearestSlot.name} (距離: {closestDistance:F1})");
        }
    }
    
    private void ClearAllHighlights()
    {
        DropSlot[] allSlots = FindObjectsByType<DropSlot>(FindObjectsSortMode.None);
        foreach (DropSlot slot in allSlots)
        {
            slot.SetSnapHighlight(false);
        }
    }
}

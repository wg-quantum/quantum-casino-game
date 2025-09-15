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
    
    // ドラッグ開始時の情報
    private bool wasInSlot = false;
    private DropSlot sourceSlot = null;
    
    // 真の元の位置（最初にゲームが開始された時の位置）
    private Vector2 trueOriginalPosition;
    private Transform trueOriginalParent;
    private bool originalPositionSaved = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        
        // 真の元の位置を保存（最初の1回だけ）
        SaveTrueOriginalPosition();
        
        Debug.Log($"DraggableItem初期化: {gameObject.name}");
        Debug.Log($"真の元の位置: {trueOriginalPosition}, 親: {trueOriginalParent?.name}");
    }
    
    private void SaveTrueOriginalPosition()
    {
        if (!originalPositionSaved)
        {
            trueOriginalPosition = rectTransform.anchoredPosition;
            trueOriginalParent = transform.parent;
            originalPositionSaved = true;
            
            Debug.Log($"真の元の位置を保存: {gameObject.name} at {trueOriginalPosition}");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"ドラッグ開始: {gameObject.name}");
        
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        
        // ドラッグ開始時にスロット内にいるかチェック
        sourceSlot = originalParent.GetComponent<DropSlot>();
        wasInSlot = (sourceSlot != null);
        
        if (wasInSlot)
        {
            // スロットから一時的に削除
            sourceSlot.RemoveItem();
            Debug.Log($"スロットから一時的に削除: {sourceSlot.name}");
        }
        
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
        DropSlot targetSlot = null;
        
        // 1. 直接ドロップを試行
        if (eventData.pointerEnter != null)
        {
            targetSlot = eventData.pointerEnter.GetComponent<DropSlot>();
        }
        
        if (targetSlot != null)
        {
            Debug.Log($"直接ドロップ成功: {targetSlot.name}");
            targetSlot.PlaceItem(gameObject);
            dropSuccessful = true;
        }
        // 2. スナップを試行
        else if (nearestSlot != null)
        {
            Debug.Log($"スナップドロップ成功: {nearestSlot.name}");
            nearestSlot.PlaceItem(gameObject);
            dropSuccessful = true;
        }
        
        // 3. ドロップに失敗した場合の処理
        if (!dropSuccessful)
        {
            if (wasInSlot)
            {
                // 元々スロットにあった場合の処理
                if (IsDroppedOutsideAllSlots(eventData))
                {
                    // スロット外にドロップした場合：真の元の位置に戻す
                    ReturnToTrueOriginalPosition();
                    Debug.Log("スロット外にドロップ - 真の元の位置に戻します");
                }
                else
                {
                    // 無効な場所の場合：元のスロットに戻す
                    ReturnToOriginalSlot();
                    Debug.Log("無効な場所 - 元のスロットに戻します");
                }
            }
            else
            {
                // 元々自由な場所にあった場合
                if (IsDroppedOutsideAllSlots(eventData))
                {
                    // スロット外にドロップ：真の元の位置に戻す
                    ReturnToTrueOriginalPosition();
                    Debug.Log("スロット外にドロップ - 真の元の位置に戻します");
                }
                else
                {
                    // 無効な場所：直前の位置に戻す
                    transform.SetParent(originalParent);
                    rectTransform.anchoredPosition = originalPosition;
                    Debug.Log("無効な場所 - 直前の位置に戻します");
                }
            }
        }
        
        ClearAllHighlights();
        nearestSlot = null;
        
        // 状態をリセット
        wasInSlot = false;
        sourceSlot = null;
    }
    
    private bool IsDroppedOutsideAllSlots(PointerEventData eventData)
    {
        // ドロップした場所がUI要素でない、またはDropSlotでない場合
        if (eventData.pointerEnter == null)
        {
            return true; // UI外にドロップ
        }
        
        // DropSlotでもDropAreaでもない場合
        DropSlot slot = eventData.pointerEnter.GetComponent<DropSlot>();
        DropAreaHandler dropArea = eventData.pointerEnter.GetComponent<DropAreaHandler>();
        
        return (slot == null && dropArea == null);
    }
    
    private void ReturnToTrueOriginalPosition()
    {
        // 真の元の位置に戻す
        if (trueOriginalParent != null)
        {
            transform.SetParent(trueOriginalParent);
            rectTransform.anchoredPosition = trueOriginalPosition;
            Debug.Log($"真の元の位置に復帰: {gameObject.name} at {trueOriginalPosition}");
        }
        else
        {
            // フォールバック：Canvas直下に配置
            transform.SetParent(canvas.transform);
            rectTransform.anchoredPosition = trueOriginalPosition;
            Debug.LogWarning($"真の元の親が見つからないため、Canvas直下に配置: {gameObject.name}");
        }
    }
    
    private void ReturnToOriginalSlot()
    {
        if (sourceSlot != null)
        {
            transform.SetParent(sourceSlot.transform);
            rectTransform.anchoredPosition = Vector2.zero;
            sourceSlot.SetCurrentItem(gameObject);
        }
        else
        {
            // フォールバック
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = originalPosition;
        }
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
    
    private float GetUIDistance(RectTransform rect1, RectTransform rect2)
    {
        Vector3 pos1 = rect1.position;
        Vector3 pos2 = rect2.position;
        
        float worldDistance = Vector3.Distance(pos1, pos2);
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
    
    // 外部から真の元の位置を再設定する場合（必要に応じて）
    public void SetTrueOriginalPosition(Vector2 position, Transform parent)
    {
        trueOriginalPosition = position;
        trueOriginalParent = parent;
        originalPositionSaved = true;
        
        Debug.Log($"真の元の位置を手動設定: {gameObject.name} at {position}");
    }
    
    // 現在の位置を新しい元の位置として保存
    public void UpdateTrueOriginalPosition()
    {
        trueOriginalPosition = rectTransform.anchoredPosition;
        trueOriginalParent = transform.parent;
        
        Debug.Log($"真の元の位置を更新: {gameObject.name} at {trueOriginalPosition}");
    }
    
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, snapDistance * canvas.scaleFactor);
        }
    }
}

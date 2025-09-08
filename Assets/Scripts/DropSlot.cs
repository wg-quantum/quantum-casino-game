using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("色設定")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverHighlightColor = Color.yellow;
    [SerializeField] private Color snapHighlightColor = Color.green;
    
    private Image image;
    private GameObject currentItem;
    private bool isHoverHighlighted = false;
    private bool isSnapHighlighted = false;
    
    void Start()
    {
        image = GetComponent<Image>();
        if (image != null)
        {
            image.color = normalColor;
        }
        
        Debug.Log($"DropSlot初期化: {gameObject.name}");
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"OnDrop呼び出し: {eventData.pointerDrag?.name} → {gameObject.name}");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"PointerEnter: {gameObject.name}");
        if (eventData.pointerDrag != null && !HasItem())
        {
            isHoverHighlighted = true;
            UpdateColor();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"PointerExit: {gameObject.name}");
        isHoverHighlighted = false;
        UpdateColor();
    }

    public void PlaceItem(GameObject item)
    {
        if (currentItem != null)
        {
            Debug.Log("既にアイテムがあります");
            return;
        }

        Debug.Log($"アイテム配置: {item.name} → {gameObject.name}");
        
        // 前のスロットから削除
        if (item.transform.parent != null)
        {
            DropSlot previousSlot = item.transform.parent.GetComponent<DropSlot>();
            if (previousSlot != null && previousSlot != this)
            {
                previousSlot.RemoveItem();
            }
        }
        
        item.transform.SetParent(transform);
        item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        
        currentItem = item;
        
        // ハイライトをクリア
        SetSnapHighlight(false);
    }

    public bool HasItem()
    {
        return currentItem != null;
    }

    public void RemoveItem()
    {
        Debug.Log($"アイテム削除: {gameObject.name}");
        currentItem = null;
    }
    
    // スナップハイライトの設定
    public void SetSnapHighlight(bool highlight)
    {
        isSnapHighlighted = highlight;
        UpdateColor();
    }
    
    private void UpdateColor()
    {
        if (image != null)
        {
            if (isSnapHighlighted)
            {
                image.color = snapHighlightColor; // スナップ範囲（緑）
            }
            else if (isHoverHighlighted)
            {
                image.color = hoverHighlightColor; // ホバー（黄色）
            }
            else
            {
                image.color = normalColor; // 通常
            }
        }
    }
}

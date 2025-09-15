using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("色設定")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverHighlightColor = Color.yellow;
    [SerializeField] private Color snapHighlightColor = Color.green;
    [SerializeField] private Color occupiedHighlightColor = new Color(1f, 0.5f, 0f, 1f); // オレンジ色
    
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
        if (eventData.pointerDrag != null)
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
        Debug.Log($"アイテム配置試行: {item.name} → {gameObject.name}");
        
        // 既存のアイテムを元の場所に戻す
        if (currentItem != null)
        {
            Debug.Log($"既存アイテムを置き換え: {currentItem.name} → {item.name}");
            ReturnItemToOriginalPosition(currentItem);
        }
        
        // 前のスロットから削除
        if (item.transform.parent != null)
        {
            DropSlot previousSlot = item.transform.parent.GetComponent<DropSlot>();
            if (previousSlot != null && previousSlot != this)
            {
                previousSlot.RemoveItem();
            }
        }
        
        // 新しいアイテムを配置
        item.transform.SetParent(transform);
        item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        
        currentItem = item;
        Debug.Log($"アイテム配置完了: {item.name} → {gameObject.name}");
        
        // ハイライトをクリア
        SetSnapHighlight(false);
    }
    
    private void ReturnItemToOriginalPosition(GameObject item)
    {
        // ItemOriginalPosition コンポーネントがあれば元の位置に戻す
        ItemOriginalPosition originalPos = item.GetComponent<ItemOriginalPosition>();
        if (originalPos != null)
        {
            originalPos.ReturnToOriginalPosition();
            Debug.Log($"既存アイテムを元の位置に戻しました: {item.name}");
        }
        else
        {
            // コンポーネントがない場合は適当な位置に配置
            item.transform.SetParent(transform.parent);
            RectTransform itemRect = item.GetComponent<RectTransform>();
            itemRect.anchoredPosition = new Vector2(
                Random.Range(-200f, 200f), 
                Random.Range(-200f, 200f)
            );
            
            Debug.Log($"既存アイテムをランダム位置に移動: {item.name}");
        }
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
    
    // 外部からcurrentItemを設定（ドラッグ失敗時の復元用）
    public void SetCurrentItem(GameObject item)
    {
        currentItem = item;
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
                // アイテムがある場合とない場合で色を変える
                if (HasItem())
                {
                    image.color = occupiedHighlightColor; // 置き換え可能（オレンジ）
                }
                else
                {
                    image.color = snapHighlightColor; // 空きスロット（緑）
                }
            }
            else if (isHoverHighlighted)
            {
                // ホバー時も同様に色分け
                if (HasItem())
                {
                    image.color = occupiedHighlightColor; // 置き換え可能（オレンジ）
                }
                else
                {
                    image.color = hoverHighlightColor; // 空きスロット（黄色）
                }
            }
            else
            {
                image.color = normalColor; // 通常
            }
        }
    }
}

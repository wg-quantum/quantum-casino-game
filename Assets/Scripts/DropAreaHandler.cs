using UnityEngine;
using UnityEngine.EventSystems;

public class DropAreaHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public DropSlot parentSlot;

    public void OnDrop(PointerEventData eventData)
    {
        if (parentSlot != null)
        {
            parentSlot.OnDrop(eventData);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (parentSlot != null)
        {
            parentSlot.OnPointerEnter(eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (parentSlot != null)
        {
            parentSlot.OnPointerExit(eventData);
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private string _itemName;
    private GameObject _prefab;
    private InventorySlot _mySlot;

    public void Initialize(string itemName, GameObject prefab, InventorySlot slot)
    {
        _itemName = itemName;
        _prefab = prefab;
        _mySlot = slot;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        InventorySlot slot = GetComponentInParent<InventorySlot>();
        if (slot != null)
            ItemDragger.Instance.StartDrag(_prefab, _itemName, slot, transform);
    }

    public void OnDrag(PointerEventData eventData) { }
    public void OnEndDrag(PointerEventData eventData) { }
}

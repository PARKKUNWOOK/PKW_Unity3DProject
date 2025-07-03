using UnityEngine;

public class ItemDragger : MonoBehaviour
{
    public static ItemDragger Instance;

    private GameObject _dragItem;
    private RectTransform _dragItemRect;
    private string _dragItemName;
    private InventorySlot _originSlot;
    private Transform _originItem;
    private ItemQuickSlot _targetQuickSlot;

    public bool IsDragging => _dragItem != null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (_dragItem != null)
        {
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform as RectTransform,
                Input.mousePosition,
                null,
                out mousePos);

            _dragItemRect.anchoredPosition = mousePos;

            if (Input.GetMouseButtonUp(0))
            {
                TryDrop();
            }
        }
    }

    public void StartDrag(GameObject originalPrefab, string itemName, InventorySlot originSlot, Transform originItem)
    {
        _originSlot = originSlot;
        _originItem = originItem;
        _dragItemName = itemName;

        _dragItem = Instantiate(originalPrefab, transform);
        _dragItem.name = "DragIcon_" + itemName;

        _dragItemRect = _dragItem.GetComponent<RectTransform>();
        _dragItemRect.anchorMin = new Vector2(0.5f, 0.5f);
        _dragItemRect.anchorMax = new Vector2(0.5f, 0.5f);
        _dragItemRect.pivot = new Vector2(0.5f, 0.5f);
        _dragItemRect.localScale = Vector3.one * 0.1f;
    }

    private void TryDrop()
    {
        foreach (var slot in FindObjectsOfType<InventorySlot>())
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(slot.GetComponent<RectTransform>(), Input.mousePosition))
            {
                HandleDropOnSlot(slot);
                return;
            }
        }

        foreach (var quickSlot in FindObjectsOfType<ItemQuickSlot>())
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(quickSlot.GetComponent<RectTransform>(), Input.mousePosition))
            {
                HandleDropOnQuickSlot(quickSlot);
                return;
            }
        }

        CancelDrag();
    }

    private void HandleDropOnSlot(InventorySlot targetSlot)
    {
        if (_originSlot == targetSlot)
        {
            CancelDrag();
            return;
        }

        if (_originItem == null)
        {
            CancelDrag();
            return;
        }

        Transform targetItem = targetSlot.HasItem() ? targetSlot.GetItem() : null;

        if (targetItem != null)
        {
            targetItem.SetParent(_originSlot.transform);
            ResetRect(targetItem.GetComponent<RectTransform>());

            _originItem.SetParent(targetSlot.transform);
            ResetRect(_originItem.GetComponent<RectTransform>());
        }
        else
        {
            _originItem.SetParent(targetSlot.transform);
            ResetRect(_originItem.GetComponent<RectTransform>());
        }

        CancelDrag();
    }

    private void HandleDropOnQuickSlot(ItemQuickSlot quickSlot)
    {
        quickSlot.AssignItem(_dragItemName);

        CancelDrag();
    }

    private void CancelDrag()
    {
        if (_dragItem != null)
            Destroy(_dragItem);

        _dragItem = null;
        _dragItemName = null;
        _originSlot = null;
        _originItem = null;
    }

    private void ResetRect(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localScale = Vector3.one;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemQuickSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject curItemTextPrefab;
    private GameObject _iconObject;
    private string _assignedItem;
    private GameObject _dragPreview;

    private void Start()
    {
        ItemQuickSlotManager.Instance.RegisterSlot(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_iconObject == null) return;

        _dragPreview = Instantiate(_iconObject, transform.parent.parent);
        RectTransform rt = _dragPreview.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.localScale = new Vector3(1.2f, 1.2f, 1f);

        TextMeshProUGUI text = _dragPreview.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            Destroy(text.gameObject);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_dragPreview != null)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.root as RectTransform,
                Input.mousePosition,
                null,
                out pos);
            _dragPreview.GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_dragPreview != null)
        {
            Destroy(_dragPreview);
        }

        foreach (var slot in FindObjectsOfType<ItemQuickSlot>())
        {
            if (slot == this) continue;

            if (RectTransformUtility.RectangleContainsScreenPoint(
                slot.GetComponent<RectTransform>(),
                Input.mousePosition))
            {
                string thisItem = _assignedItem;
                string targetItem = slot._assignedItem;

                GameObject iconPrefab = ItemIcon.Instance.GetItemPrefab(thisItem);

                slot.AssignItem(thisItem, iconPrefab);
                this.AssignItem(targetItem);

                return;
            }
        }
    }

    public void AssignItem(string itemName)
    {
        GameObject prefab = ItemIcon.Instance.GetItemPrefab(itemName);
        AssignItem(itemName, prefab);
    }

    public void AssignItem(string itemName, GameObject prefab)
    {
        if (prefab == null)
        {
            return;
        }

        ItemQuickSlotManager.Instance.UnassignItemFromOtherSlots(itemName, this);

        _assignedItem = itemName;

        if (_iconObject != null)
        {
            Destroy(_iconObject);
        }

        _iconObject = Instantiate(prefab, transform);
        _iconObject.name = itemName + "Icon(Clone)";

        _iconObject.transform.SetAsFirstSibling();

        RectTransform rt = _iconObject.GetComponent<RectTransform>();

        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(70f, 70f);
        rt.localScale = Vector3.one;

        int count = ItemIcon.Instance.GetItemCount(itemName);
        if (count > 1)
        {
            CreateCountText(_iconObject.transform, count);
        }
    }

    private void CreateCountText(Transform parent, int count)
    {
        GameObject textObj = Instantiate(curItemTextPrefab, parent);
        textObj.name = "CurItemText(Clone)";
        TextMeshProUGUI text = textObj.GetComponent<TextMeshProUGUI>();
        text.text = count.ToString();

        RectTransform rt = textObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 1f);
        rt.anchoredPosition = new Vector2(5.7f, 0f);
        rt.sizeDelta = new Vector2(30f, 30f);
        rt.localScale = Vector3.one;
    }

    public bool HasItem(string itemName)
    {
        return _assignedItem == itemName;
    }

    public void UpdateCountText(int count)
    {
        TextMeshProUGUI[] allTexts = _iconObject.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var t in allTexts)
        {
            if (t.gameObject.name.Contains("Clone") || t.gameObject.name.Contains("CurItemText"))
            {
                Destroy(t.gameObject);
            }
        }

        if (count > 1)
        {
            CreateCountText(_iconObject.transform, count);
        }
    }

    public void ClearSlot()
    {
        _assignedItem = null;

        if (_iconObject != null)
        {
            Destroy(_iconObject);
            _iconObject = null;
        }
    }

    public string GetItemName()
    {
        return _assignedItem;
    }
}

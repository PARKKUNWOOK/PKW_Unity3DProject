using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
    public static ItemIcon Instance;

    [SerializeField] private GameObject hpPotionPrefab;
    [SerializeField] private GameObject mpPotionPrefab;
    [SerializeField] private GameObject curItemTextPrefab;

    [Header("Slot Root")]
    [SerializeField] private Transform contentParent;

    private List<Transform> _inventorySlots = new();
    private Dictionary<string, int> _itemCounts = new();
    private Dictionary<string, GameObject> _itemIcons = new();
    private Dictionary<string, InventorySlot> _itemSlots = new();
    private Dictionary<string, int> _itemCount = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (Transform child in contentParent)
        {
            _inventorySlots.Add(child);
        }
    }

    public void AddItem(string itemName)
    {
        bool iconExists = _itemIcons.ContainsKey(itemName) && _itemIcons[itemName] != null;

        if (!_itemCounts.ContainsKey(itemName))
            _itemCounts[itemName] = 0;

        _itemCounts[itemName]++;

        if (iconExists)
        {
            UpdateItemCountUI(itemName);
        }
        else
        {
            GameObject prefab = GetItemPrefab(itemName);
            if (prefab == null) return;

            Transform slot = FindFirstEmptySlot();
            if (slot == null)
            {
                Debug.LogWarning("빈 인벤토리 슬롯이 없습니다.");
                return;
            }

            GameObject icon = Instantiate(prefab, slot);
            _itemIcons[itemName] = icon;

            RectTransform rt = icon.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.localScale = Vector3.one;

            InventorySlot slotComponent = slot.GetComponent<InventorySlot>();
            RegisterSlot(itemName, slotComponent);

            DraggableItem drag = icon.AddComponent<DraggableItem>();
            drag.Initialize(itemName, prefab, slotComponent);

            if (_itemCounts[itemName] > 1)
                CreateCountText(slot, itemName);
        }

        ItemQuickSlot[] quickSlots = FindObjectsOfType<ItemQuickSlot>();
        foreach (var slot in quickSlots)
        {
            if (slot.GetItemName() == itemName)
            {
                int count = GetItemCount(itemName);
                slot.UpdateCountText(count);
            }
        }
    }

    public void DecreaseItemCount(string itemName)
    {
        if (!_itemCounts.ContainsKey(itemName)) return;

        _itemCounts[itemName] = Mathf.Max(0, _itemCounts[itemName] - 1);
        UpdateItemCountUI(itemName);

        if (_itemCounts[itemName] == 0)
        {
            if (_itemIcons.ContainsKey(itemName))
            {
                GameObject icon = _itemIcons[itemName];
                if (icon != null)
                    Destroy(icon);

                _itemIcons.Remove(itemName);
            }

            _itemCounts.Remove(itemName);
            _itemSlots.Remove(itemName);
        }
    }

    public GameObject GetItemPrefab(string itemName)
    {
        return itemName switch
        {
            "HpPotion" => hpPotionPrefab,
            "MpPotion" => mpPotionPrefab,
            _ => null
        };
    }

    private Transform FindFirstEmptySlot()
    {
        foreach (Transform slot in _inventorySlots)
        {
            if (slot.childCount == 0)
                return slot;
        }
        return null;
    }

    private void CreateCountText(Transform slot, string itemName)
    {
        GameObject textObj = Instantiate(curItemTextPrefab, slot);
        TextMeshProUGUI text = textObj.GetComponent<TextMeshProUGUI>();
        text.text = _itemCounts[itemName].ToString();

        RectTransform rt = textObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 1f);
        rt.anchoredPosition = new Vector2(1f, -96.5f);
        rt.sizeDelta = new Vector2(30f, 30f);
        rt.localScale = new Vector3(3.2f, 2f, 2f);
    }

    private void UpdateItemCountUI(string itemName)
    {
        if (!_itemIcons.ContainsKey(itemName)) return;

        GameObject icon = _itemIcons[itemName];
        if (icon == null)
        {
            _itemIcons.Remove(itemName);
            return;
        }

        TextMeshProUGUI text = icon.GetComponentInChildren<TextMeshProUGUI>();

        if (_itemCounts[itemName] > 1)
        {
            if (text == null)
            {
                CreateCountText(icon.transform, itemName);
            }
            else
            {
                text.text = _itemCounts[itemName].ToString();
            }
        }
        else
        {
            if (text != null)
                Destroy(text.gameObject);
        }
    }
    
    public InventorySlot GetSlotByItemName(string itemName)
    {
        if (_itemSlots.TryGetValue(itemName, out InventorySlot slot))
            return slot;
        return null;
    }

    public void RegisterSlot(string itemName, InventorySlot slot)
    {
        _itemSlots[itemName] = slot;
    }

    public int GetItemCount(string itemName)
    {
        return _itemCounts.TryGetValue(itemName, out int count) ? count : 0;
    }

    public void UpdateAllInventorySlotTexts()
    {
        foreach (InventorySlot slot in FindObjectsOfType<InventorySlot>())
        {
            slot.UpdateCountText();
        }
    }
}

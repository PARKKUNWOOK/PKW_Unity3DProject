using System.Collections.Generic;
using UnityEngine;

public class ItemQuickSlotManager : MonoBehaviour
{
    public static ItemQuickSlotManager Instance;

    private List<ItemQuickSlot> _quickSlots = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) UseQuickSlotItem(0);
        else if (Input.GetKeyDown(KeyCode.X)) UseQuickSlotItem(4);
        else if (Input.GetKeyDown(KeyCode.Alpha7)) UseQuickSlotItem(2);
        else if (Input.GetKeyDown(KeyCode.Alpha8)) UseQuickSlotItem(3);
        else if (Input.GetKeyDown(KeyCode.Alpha9)) UseQuickSlotItem(5);
        else if (Input.GetKeyDown(KeyCode.Alpha0)) UseQuickSlotItem(1);
    }

    public void RegisterSlot(ItemQuickSlot slot)
    {
        if (!_quickSlots.Contains(slot))
            _quickSlots.Add(slot);
    }

    public void UnassignItemFromOtherSlots(string itemName, ItemQuickSlot exceptSlot)
    {
        foreach (var slot in _quickSlots)
        {
            if (slot == exceptSlot) continue;

            if (slot.HasItem(itemName))
            {
                slot.ClearSlot();
            }
        }
    }

    public void UseQuickSlotItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _quickSlots.Count)
            return;

        ItemQuickSlot slot = _quickSlots[slotIndex];
        string itemName = slot.GetItemName();

        if (string.IsNullOrEmpty(itemName)) return;

        PlayerKnight player = PlayerKnight.Instance;

        if (itemName == "HpPotion")
        {
            if (player == null || player.CurHp >= player.MaxHp) return;

            player.HealHp(70);
        }
        else if (itemName == "MpPotion")
        {
            if (player == null || player.CurMp >= player.MaxMp) return;

            player.HealMp(10);
        }
        else return;

        ItemIcon.Instance.DecreaseItemCount(itemName);

        if (ItemIcon.Instance.GetItemCount(itemName) <= 0)
        {
            slot.ClearSlot();

            foreach (InventorySlot invSlot in FindObjectsOfType<InventorySlot>())
            {
                if (!invSlot.HasItem()) continue;

                Transform child = invSlot.GetItem();
                if (child.name.StartsWith(itemName))
                {
                    invSlot.ClearItem();
                }
            }
        }
        else
        {
            int currentCount = ItemIcon.Instance.GetItemCount(itemName);
            slot.UpdateCountText(currentCount);
        }

        ItemIcon.Instance.UpdateAllInventorySlotTexts();
    }
}

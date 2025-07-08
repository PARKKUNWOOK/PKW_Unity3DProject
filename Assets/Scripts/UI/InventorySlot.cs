using TMPro;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public bool HasItem()
    {
        return transform.childCount > 0;
    }

    public Transform GetItem()
    {
        return HasItem() ? transform.GetChild(0) : null;
    }

    public void UpdateCountText()
    {
        if (!HasItem()) return;

        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            string itemName = transform.GetChild(0).name.Replace("Icon(Clone)", "");
            int count = ItemIcon.Instance.GetItemCount(itemName);

            if (count > 1)
                text.text = count.ToString();
            else
                Destroy(text.gameObject);
        }
    }

    public void ClearItem()
    {
        if (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            Destroy(child.gameObject);
        }
    }
}

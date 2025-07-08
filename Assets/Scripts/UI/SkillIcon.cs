using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject iconPrefab;
    private GameObject draggingIcon;
    private RectTransform draggingRect;

    private Canvas canvas;

    public int skillKey;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        PlayerSkillData skillData = PlayerSkillDataManager.Instance.GetPlayerSkillData(skillKey);

        if (skillData.CurLevel <= 0)
        {
            return;
        }

        draggingIcon = Instantiate(iconPrefab, canvas.transform);
        draggingIcon.GetComponent<Image>().sprite = GetComponent<Image>().sprite;

        draggingRect = draggingIcon.GetComponent<RectTransform>();
        draggingRect.localScale = new Vector3(0.029f, 0.048f, 1f);

        draggingIcon.transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggingIcon == null)
            return;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pos);

        draggingRect.anchoredPosition = pos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggingIcon == null)
            return;

        GameObject target = GetSlotUnderPointer(eventData);
        if (target != null)
        {
            RemoveDuplicateIcons();
            ClearPreviousSlot();

            foreach (Transform child in target.transform)
            {
                if (child.CompareTag("SkillIcon"))
                {
                    Destroy(child.gameObject);
                }
            }

            draggingIcon.transform.SetParent(target.transform, false);

            draggingRect.anchorMin = new Vector2(0.5f, 0.5f);
            draggingRect.anchorMax = new Vector2(0.5f, 0.5f);
            draggingRect.pivot = new Vector2(0.5f, 0.5f);

            draggingRect.anchoredPosition = Vector2.zero;

            draggingRect.anchoredPosition = new Vector2(0.1f, 1f);
            draggingRect.sizeDelta = new Vector2(2569f, 1522f);

            SkillSlot slot = target.GetComponent<SkillSlot>();
            if (slot != null)
            {
                slot.AssignSkill(skillKey);
            }

            draggingIcon.transform.SetSiblingIndex(0);
        }
        else
        {
            Destroy(draggingIcon);
        }

        draggingIcon = null;
    }

    private void RemoveDuplicateIcons()
    {
        GameObject[] slots = GameObject.FindGameObjectsWithTag("SkillSlot");

        foreach (GameObject slot in slots)
        {
            foreach (Transform child in slot.transform)
            {
                Image iconImage = child.GetComponent<Image>();
                if (iconImage != null && iconImage.sprite == GetComponent<Image>().sprite)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    private void ClearPreviousSlot()
    {
        GameObject[] slots = GameObject.FindGameObjectsWithTag("SkillSlot");

        foreach (GameObject slotObj in slots)
        {
            SkillSlot slot = slotObj.GetComponent<SkillSlot>();
            if (slot != null && slot.AssignedSkillKey == skillKey)
            {
                slot.ClearSlot();
            }
        }
    }

    private GameObject GetSlotUnderPointer(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.CompareTag("SkillSlot"))
            {
                return result.gameObject;
            }
        }
        return null;
    }
}

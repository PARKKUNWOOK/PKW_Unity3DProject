using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GetTextPopupManager : MonoBehaviour
{
    public static GetTextPopupManager Instance;

    [Header("텍스트 프리팹")]
    [SerializeField] private GameObject expTextPrefab;
    [SerializeField] private GameObject goldTextPrefab;
    [SerializeField] private GameObject itemTextPrefab;

    [Header("부모 오브젝트")]
    [SerializeField] private Transform textParent;

    [Header("설정")]
    [SerializeField] private float riseHeight = 30f;
    [SerializeField] private float riseTime = 0.2f;
    [SerializeField] private float fadeTime = 1.0f;
    [SerializeField] private Vector3 offset = new Vector3(1.5f, 2f, 0);

    private HashSet<CanvasGroup> _fadingOutGroups = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowExpGoldAndItems(float exp, int gold, Dictionary<string, int> items)
    {
        var popupList = new List<(string, float?, int?)>
        {
            ("Exp", exp, null),
            ("Gold", null, gold)
        };

        if (items.TryGetValue("HpPotion", out int hpCount) && hpCount > 0)
            popupList.Add(("HpPotion", null, hpCount));

        if (items.TryGetValue("MpPotion", out int mpCount) && mpCount > 0)
            popupList.Add(("MpPotion", null, mpCount));

        StartCoroutine(ShowSequence(popupList));
    }

    private IEnumerator ShowSequence(List<(string type, float? exp, int? count)> sequence)
    {
        Vector3 basePos = Camera.main.WorldToScreenPoint(PlayerKnight.Instance.transform.position + offset);
        float verticalGap = 35f;
        float displayDuration = 0.4f;

        List<GameObject> activeTexts = new();
        List<CanvasGroup> activeGroups = new();

        for (int i = 0; i < sequence.Count; i++)
        {
            var entry = sequence[i];

            GameObject prefab = GetPrefab(entry.type);
            GameObject obj = Instantiate(prefab, textParent);
            obj.transform.position = basePos + new Vector3(0, -verticalGap * i, 0); // 아래로 쌓임

            CanvasGroup group = obj.GetComponent<CanvasGroup>() ?? obj.AddComponent<CanvasGroup>();
            group.alpha = 1f;

            SetupTextUI(obj, entry);

            activeTexts.Add(obj);
            activeGroups.Add(group);

            // 올라가기 애니메이션
            Vector3 startPos = obj.transform.position;
            Vector3 targetPos = startPos + new Vector3(0, riseHeight, 0);
            float t = 0f;
            while (t < riseTime)
            {
                obj.transform.position = Vector3.Lerp(startPos, targetPos, t / riseTime);
                t += Time.deltaTime;
                yield return null;
            }
            obj.transform.position = targetPos;

            // 이전 텍스트 페이드 아웃
            if (i > 0)
            {
                CanvasGroup prev = activeGroups[i - 1];
                if (prev != null)
                    StartCoroutine(FadeOut(prev, fadeTime));

                yield return new WaitUntil(() => prev == null || prev.alpha <= 0.5f);

                if (group != null)
                    StartCoroutine(FadeOut(group, fadeTime));
            }

            yield return new WaitForSeconds(displayDuration);
        }

        if (activeGroups.Count > 0)
        {
            CanvasGroup last = activeGroups[^1];
            if (last != null)
                StartCoroutine(FadeOut(last, fadeTime));
        }
    }

    private GameObject GetPrefab(string type)
    {
        return type switch
        {
            "Exp" => expTextPrefab,
            "Gold" => goldTextPrefab,
            _ => itemTextPrefab
        };
    }

    private void SetupTextUI(GameObject obj, (string type, float? exp, int? count) data)
    {
        var text = obj.GetComponentInChildren<TextMeshProUGUI>();
        if (text == null) return;

        if (data.type == "Exp")
        {
            string prefix = text.text.Split(' ')[0];
            text.text = $"{prefix} {data.exp.Value:0.##}";
        }
        else if (data.type == "Gold")
        {
            string prefix = text.text.Split(' ')[0];
            text.text = $"{prefix} {data.count.Value}";
        }
        else
        {
            string displayName = GetLocalizedItemName(data.type);
            int count = data.count.HasValue ? data.count.Value : 1;
            text.text = $"{displayName} {count}";

            Image image = obj.GetComponentInChildren<Image>();
            GameObject iconPrefab = ItemIcon.Instance.GetItemPrefab(data.type);
            if (iconPrefab != null)
            {
                Image source = iconPrefab.GetComponentInChildren<Image>();
                if (image != null && source != null)
                    image.sprite = source.sprite;
            }
        }
    }

    private string GetLocalizedItemName(string key)
    {
        return key switch
        {
            "HpPotion" => "HpPotion",
            "MpPotion" => "MpPotion",
            _ => key
        };
    }

    private IEnumerator FadeOut(CanvasGroup group, float duration)
    {
        if (group == null || _fadingOutGroups.Contains(group)) yield break;

        _fadingOutGroups.Add(group);

        float t = 0f;
        while (t < duration && group != null)
        {
            group.alpha = 1f - (t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        if (group != null)
        {
            group.alpha = 0f;
            if (group.gameObject != null)
                Destroy(group.gameObject);
        }

        _fadingOutGroups.Remove(group);
    }
}

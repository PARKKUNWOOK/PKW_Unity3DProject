using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillLevelButton : MonoBehaviour
{
    public int skillKey;
    public TextMeshProUGUI skillLevelText;
    public Button plusButton;
    public Button minusButton;
    public Button resetButton;
    public Button checkButton;

    private PlayerSkillData skillData;
    private int tempLevel;
    private int savedLevel;

    private void Start()
    {
        skillData = PlayerSkillDataManager.Instance.GetPlayerSkillData(skillKey);
        savedLevel = skillData.CurLevel;
        tempLevel = savedLevel;

        plusButton.onClick.AddListener(OnClickPlus);
        minusButton.onClick.AddListener(OnClickMinus);
        resetButton.onClick.AddListener(OnClickReset);
        checkButton.onClick.AddListener(OnClickCheck);

        UpdateSkillLevelText();
    }

    private void OnClickPlus()
    {
        if (tempLevel >= skillData.MaxLevel)
        {
            Debug.Log($"{skillData.Name} ��ų�� �ִ� �����Դϴ�.");
            return;
        }

        if (SkillWindow.Instance.SkillLevelUp(1))
        {
            tempLevel++;
            UpdateSkillLevelText();
        }
    }

    private void OnClickMinus()
    {
        if (tempLevel <= savedLevel)
        {
            Debug.Log("����� ���� ���Ϸ� ������ �� �����ϴ�.");
            return;
        }

        tempLevel--;
        SkillWindow.Instance.AddSkillPoint(1);
        UpdateSkillLevelText();
    }

    private void OnClickReset()
    {
        int refundPoint = tempLevel - savedLevel;
        if (refundPoint > 0)
        {
            tempLevel = savedLevel;
            SkillWindow.Instance.AddSkillPoint(refundPoint);
            UpdateSkillLevelText();
            Debug.Log($"{skillData.Name} ��ų ����! ����Ʈ {refundPoint} ��ȯ");
        }
    }

    private void OnClickCheck()
    {
        savedLevel = tempLevel;
        PlayerSkillDataManager.Instance.UpdateSkillLevel(skillKey, savedLevel);
        Debug.Log($"{skillData.Name} ��ų ���� Ȯ��: Lv.{savedLevel}");
    }

    private void UpdateSkillLevelText()
    {
        skillLevelText.text = $"Lv.{tempLevel}";
    }
}

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
        if (PlayerKnight.Instance == null)
            return;

        if (PlayerKnight.Instance.Level < skillData.PlayerLevel)
        {
            Debug.Log($"{skillData.Name} ��ų�� �÷��̾� ���� {skillData.PlayerLevel} �̻��̾�� ������ �� �ֽ��ϴ�.");
            return;
        }

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

        PlayerSkillData currentSkillData = PlayerSkillDataManager.Instance.GetPlayerSkillData(skillKey);

        if (skillKey == 103)
        {
            PlayerSkillDataManager.Instance.UpdateSkillLevel(skillKey, savedLevel);
            Debug.Log($"{currentSkillData.Name} (���� ��ų) ��ų ���� Ȯ��: Lv.{savedLevel} (���ݷ� ���� ����)");
        }
        else
        {
            int baseAttackPower = currentSkillData.AttackPower;
            int newAttackPower = baseAttackPower * (int)Mathf.Pow(2, savedLevel - 1);

            PlayerSkillDataManager.Instance.UpdateSkillLevelAndAttackPower(skillKey, savedLevel, newAttackPower);

            Debug.Log($"{currentSkillData.Name} ��ų ���� Ȯ��: Lv.{savedLevel}, ���ݷ�: {newAttackPower}");
        }
    }

    private void UpdateSkillLevelText()
    {
        skillLevelText.text = $"Lv.{tempLevel}";
    }
}

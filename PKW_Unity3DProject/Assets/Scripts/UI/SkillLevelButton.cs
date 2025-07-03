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
            Debug.Log($"{skillData.Name} 스킬은 플레이어 레벨 {skillData.PlayerLevel} 이상이어야 습득할 수 있습니다.");
            return;
        }

        if (tempLevel >= skillData.MaxLevel)
        {
            Debug.Log($"{skillData.Name} 스킬은 최대 레벨입니다.");
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
            Debug.Log("저장된 레벨 이하로 감소할 수 없습니다.");
            return;
        }

        tempLevel--;
        SkillWindow.Instance.AddSkillPoint(1);
        UpdateSkillLevelText();
    }

    private void OnClickReset()
    {
        if (tempLevel == savedLevel || savedLevel == 0)
        {
            Debug.Log("리셋할 변경사항이 없거나 스킬 레벨이 0입니다.");
            return;
        }

        int refundPoint = tempLevel - savedLevel;
        if (refundPoint > 0)
        {
            tempLevel = savedLevel;
            SkillWindow.Instance.AddSkillPoint(refundPoint);
            UpdateSkillLevelText();
            Debug.Log($"{skillData.Name} 스킬 리셋! 포인트 {refundPoint} 반환");
        }
    }

    private void OnClickCheck()
    {
        if (savedLevel == tempLevel)
        {
            Debug.Log("변경된 레벨이 없습니다.");
            return;
        }
        if (tempLevel == 0)
        {
            Debug.Log("스킬 레벨이 0이라 저장할 수 없습니다.");
            return;
        }

        savedLevel = tempLevel;

        PlayerSkillData currentSkillData = PlayerSkillDataManager.Instance.GetPlayerSkillData(skillKey);

        if (skillKey == 103)
        {
            PlayerSkillDataManager.Instance.UpdateSkillLevel(skillKey, savedLevel);
            Debug.Log($"{currentSkillData.Name} (버프 스킬) 스킬 레벨 확정: Lv.{savedLevel} (공격력 변동 없음)");
        }
        else
        {
            int baseAttackPower = currentSkillData.AttackPower;
            int newAttackPower = baseAttackPower * (int)Mathf.Pow(2, savedLevel - 1);

            PlayerSkillDataManager.Instance.UpdateSkillLevelAndAttackPower(skillKey, savedLevel, newAttackPower);

            Debug.Log($"{currentSkillData.Name} 스킬 레벨 확정: Lv.{savedLevel}, 공격력: {newAttackPower}");
        }
    }

    private void UpdateSkillLevelText()
    {
        skillLevelText.text = $"Lv.{tempLevel}";
    }
}

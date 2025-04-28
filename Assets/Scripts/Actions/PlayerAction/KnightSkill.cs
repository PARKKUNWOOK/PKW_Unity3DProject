using System.Collections.Generic;
using UnityEngine;

public class KnightSkill : MonoBehaviour
{
    private Animator _animator;
    private KnightAttack _attack;
    private KnightBlock _block;
    private PlayerSkillData _playerSkillData;
    public static int CurrentSkillAttackPower { get; set; } = 0;
    public static int CurrentSkillKey { get; set; } = -1;

    public bool IsSkill { get; private set; }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _attack = GetComponent<KnightAttack>();
        _block = GetComponent<KnightBlock>();
    }

    private void Update()
    {
        if ((_attack != null && _attack.IsAttacking) || (_block != null && _block.IsBlocking))
            return;

        if (IsSkill)
            return;

        for (int i = 1; i <= 5; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                TryUseSkillInSlot(i);
            }
        }
    }

    private void TryUseSkillInSlot(int slotNumber)
    {
        GameObject slotObj = GameObject.Find($"SkillSlot{slotNumber}");
        if (slotObj == null) return;

        SkillSlot slot = slotObj.GetComponent<SkillSlot>();
        if (slot == null || slot.AssignedSkillKey == -1) return;

        if (slot.IsCoolingDown())
        {
            Debug.Log("��ų ��Ÿ�� ���Դϴ�.");
            return;
        }

        int skillKey = slot.AssignedSkillKey;
        PlayerSkillData skillData = PlayerSkillDataManager.Instance.GetPlayerSkillData(skillKey);

        if (skillData.CurLevel <= 0)
        {
            Debug.Log($"{skillData.Name} ��ų ������ 0�̶� ����� �� �����ϴ�.");
            return;
        }

        if (PlayerKnight.Instance.CurMp >= skillData.MpCost)
        {
            PlayerKnight.Instance.ConsumeMp(skillData.MpCost);
            _animator.SetTrigger(GetAnimationTrigger(skillKey));

            slot.StartCoolTime(skillData.CoolTime);

            Debug.Log($"{skillData.Name} ��ų ���! MP {skillData.MpCost} �Ҹ�");
        }
        else
        {
            Debug.Log($"{skillData.Name} ��ų ��� �Ұ� - MP ����");
        }

        if (skillKey != 103)
        {
            CurrentSkillAttackPower = skillData.AttackPower;
            CurrentSkillKey = skillKey;
        }
        else
        {
            CurrentSkillAttackPower = 0;
            CurrentSkillKey = -1;
        }
    }

    private string GetAnimationTrigger(int skillKey)
    {
        switch (skillKey)
        {
            case 101: return "TripleSlashSkill";
            case 102: return "JumpSkill";
            case 103: return "PowerUpSkill";
            case 104: return "SpinSlashSkill";
            case 105: return "ChargeSkill";
            default: return "";
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class KnightSkill : MonoBehaviour
{
    private Animator _animator;
    private KnightAttack _attack;
    private KnightBlock _block;
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
        if (PlayerKnight.Instance.IsGettingHit) return;

        if ((_attack != null && _attack.IsAttacking) || (_block != null && _block.IsBlocking) || IsSkill || PlayerKnight.Instance.IsDead)
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

        int finalSkillAttackPower = skillData.AttackPower;

        if (PlayerKnight.Instance.IsPowerUpBuffActive)
        {
            finalSkillAttackPower *= 2;
        }

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

            if (skillKey == 103)
            {
                PlayerKnight.Instance.ActivatePowerUpBuff(skillData.AttackPower, skillData.Duration);

                CurrentSkillAttackPower = 0;
                CurrentSkillKey = -1;

                Debug.Log($"{skillData.Name} ���� ��ų ���! ���ݷ� {skillData.AttackPower}�� ���, {skillData.Duration}�� ����");
            }
            else
            {
                CurrentSkillAttackPower = finalSkillAttackPower;
                CurrentSkillKey = skillKey;

                Debug.Log($"{skillData.Name} ��ų ���! MP {skillData.MpCost} �Ҹ�");
            }
            IsSkill = true;
        }
        else
        {
            Debug.Log($"{skillData.Name} ��ų ��� �Ұ� - MP ����");
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

    public void EndSkill()
    {
        IsSkill = false;
    }

    public void ForceStopSkill()
    {
        IsSkill = false;
        KnightSkill.CurrentSkillKey = -1;
        KnightSkill.CurrentSkillAttackPower = 0;
    }

    public void PlayTripleSlashEffect1()
    {
        PlayerEffectController effect = GetComponentInChildren<PlayerEffectController>();
        if (effect != null)
            effect.PlayTripleSlashEffect(1);
    }

    public void PlayTripleSlashEffect2()
    {
        PlayerEffectController effect = GetComponentInChildren<PlayerEffectController>();
        if (effect != null)
            effect.PlayTripleSlashEffect(2);
    }

    public void PlayTripleSlashEffect3()
    {
        PlayerEffectController effect = GetComponentInChildren<PlayerEffectController>();
        if (effect != null)
            effect.PlayTripleSlashEffect(3);
    }

    public void SpawnSkillEffect()
    {
        PlayerEffectController effect = GetComponentInChildren<PlayerEffectController>();
        if (effect != null && CurrentSkillKey != 103)
            effect.PlaySkillEffect(CurrentSkillKey);
    }

    public void StopHeavenlySlashEffect()
    {
        PlayerEffectController effect = GetComponentInChildren<PlayerEffectController>();
        if (effect != null)
            effect.StopSkillEffect(105);
    }

    public void SpawnBuffEffect()
    {
        PlayerEffectController effect = GetComponentInChildren<PlayerEffectController>();
        if (effect != null)
            effect.PlayBuffEffect();
    }

    public void StopBuffEffect()
    {
        PlayerEffectController effect = GetComponentInChildren<PlayerEffectController>();
        if (effect != null)
            effect.StopBuffEffect();
    }

    public void SpawnChargeHeavenlySlashEffect()
    {
        PlayerEffectController effect = GetComponentInChildren<PlayerEffectController>();
        if (effect != null)
            effect.PlayChargeHeavenlySlashEffect();
    }

    public void StopChargeHeavenlySlashEffect()
    {
        PlayerEffectController effect = GetComponentInChildren<PlayerEffectController>();
        if (effect != null)
            effect.StopChargeHeavenlySlashEffect();
    }
}

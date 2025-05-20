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
        if (!PlayerKnight.Instance.CanControl) return;
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
            Debug.Log("스킬 쿨타임 중입니다.");
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
            Debug.Log($"{skillData.Name} 스킬 레벨이 0이라 사용할 수 없습니다.");
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
            }
            else
            {
                CurrentSkillAttackPower = finalSkillAttackPower;
                CurrentSkillKey = skillKey;
            }
            IsSkill = true;
        }
        else
        {
            Debug.Log($"{skillData.Name} 스킬 사용 불가 - MP 부족");
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

    public void StopAllSkillEffects()
    {
        PlayerEffectController effect = GetComponentInChildren<PlayerEffectController>();
        if (effect == null) return;

        effect.StopBuffEffect();

        effect.StopSkillEffect(105);
        effect.StopChargeHeavenlySlashEffect();
    }
}

using UnityEngine;

public class PlayerEffectController : MonoBehaviour
{
    [Header("Attack Effect")]
    [SerializeField] private GameObject attackEffect;

    [Header("Triple Slash Effects")]
    [SerializeField] private GameObject tripleSlashEffect1;
    [SerializeField] private GameObject tripleSlashEffect2;
    [SerializeField] private GameObject tripleSlashEffect3;

    [Header("Skill Effects (SkillKey 기준)")]
    [SerializeField] private GameObject effect102;
    [SerializeField] private GameObject effect104;
    [SerializeField] private GameObject effect105;

    [Header("Buff Skill Effect (103번 스킬)")]
    [SerializeField] private GameObject buffEffect;

    [Header("105번 Charge 이펙트 (포즈 시 연출)")]
    [SerializeField] private GameObject chargeHeavenlySlashEffect;

    private void Start()
    {
        DeactivateAll();
    }

    private void DeactivateAll()
    {
        if (attackEffect) attackEffect.SetActive(false);

        if (tripleSlashEffect1) tripleSlashEffect1.SetActive(false);
        if (tripleSlashEffect2) tripleSlashEffect2.SetActive(false);
        if (tripleSlashEffect3) tripleSlashEffect3.SetActive(false);

        if (effect102) effect102.SetActive(false);
        if (effect104) effect104.SetActive(false);
        if (effect105) effect105.SetActive(false);

        if (buffEffect) buffEffect.SetActive(false);
        if (chargeHeavenlySlashEffect) chargeHeavenlySlashEffect.SetActive(false);
    }

    public void PlayAttackEffect()
    {
        if (attackEffect == null) return;

        attackEffect.SetActive(false);
        attackEffect.SetActive(true);
    }

    public void PlayTripleSlashEffect(int index)
    {
        switch (index)
        {
            case 1:
                if (tripleSlashEffect1)
                {
                    tripleSlashEffect1.SetActive(false);
                    tripleSlashEffect1.SetActive(true);
                }
                break;
            case 2:
                if (tripleSlashEffect2)
                {
                    tripleSlashEffect2.SetActive(false);
                    tripleSlashEffect2.SetActive(true);
                }
                break;
            case 3:
                if (tripleSlashEffect3)
                {
                    tripleSlashEffect3.SetActive(false);
                    tripleSlashEffect3.SetActive(true);
                }
                break;
        }
    }

    public void PlaySkillEffect(int skillKey)
    {
        GameObject target = null;

        switch (skillKey)
        {
            case 102: target = effect102; break;
            case 104: target = effect104; break;
            case 105: target = effect105; break;
        }

        if (target == null) return;

        target.SetActive(false);
        target.SetActive(true);
    }

    public void StopSkillEffect(int skillKey)
    {
        GameObject target = null;

        switch (skillKey)
        {
            case 105: target = effect105; break;
        }

        if (target == null) return;
        target.SetActive(false);
    }

    public void PlayBuffEffect()
    {
        if (!buffEffect) return;
        buffEffect.SetActive(false);
        buffEffect.SetActive(true);
    }

    public void StopBuffEffect()
    {
        if (!buffEffect) return;

        buffEffect.SetActive(false);
    }

    public void PlayChargeHeavenlySlashEffect()
    {
        if (!chargeHeavenlySlashEffect) return;
        chargeHeavenlySlashEffect.SetActive(false);
        chargeHeavenlySlashEffect.SetActive(true);
    }

    public void StopChargeHeavenlySlashEffect()
    {
        if (!chargeHeavenlySlashEffect) return;
        chargeHeavenlySlashEffect.SetActive(false);
    }
}

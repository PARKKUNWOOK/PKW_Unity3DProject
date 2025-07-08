using UnityEngine;

public class MeleeWeaponTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _owner;

    public void SetOwner(GameObject owner)
    {
        _owner = owner;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_owner != null && _owner.CompareTag("Monster"))
            {
                if (PlayerKnight.Instance.IsDead)
                    return;

                int baseDamage = 6;
                int finalDamage = baseDamage;


                KnightBlock block = other.GetComponent<KnightBlock>();
                if (block != null && block.IsBlocking)
                {
                    finalDamage = Mathf.CeilToInt(baseDamage * 0.1f);
                }

                PlayerKnight.Instance.GetDamage(finalDamage);

                Vector3 hitPos = other.ClosestPoint(transform.position);
                HitEffectController.Instance.PlayHitEffect(hitPos);
            }
        }

        if (other.CompareTag("Monster"))
        {
            if (_owner != null && _owner == other.gameObject)
                return;

            if (_owner != null && _owner.CompareTag("Player"))
            {
                int damage = 0;

                if (KnightSkill.CurrentSkillKey != -1)
                {
                    damage = KnightSkill.CurrentSkillAttackPower;
                }
                else
                {
                    damage = PlayerKnight.Instance.AttackPower;
                }

                Minotaurs monster = other.GetComponent<Minotaurs>();
                if (monster != null)
                {
                    monster.GetDamage(damage);

                    Vector3 hitPos = other.ClosestPoint(transform.position);
                    HitEffectController.Instance.PlayHitEffect(hitPos);
                }
            }
        }
    }
}

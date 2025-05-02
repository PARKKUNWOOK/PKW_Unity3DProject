using UnityEngine;

public class Minotaurs : MonoBehaviour
{
    //�׽�Ʈ
    private static int _globalIdCounter = 1;
    private int _minotaurId;

    /// ///////////////////////////////////////////
    
    private Animator _animator;
    private int _hp = 50;

    public bool IsDead => _hp <= 0;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _minotaurId = _globalIdCounter++;
        gameObject.name = $"Minotaurs_{_minotaurId}";

        Debug.Log($"[{gameObject.name}] ���� ü��: {_hp}");
    }

    public void GetDamage(int damage)
    {
        if (IsDead) return;

        _hp -= damage;

        Debug.Log($"[{gameObject.name}] �ǰ�! ���� ����: {damage}, ���� HP: {_hp}");

        if (_hp <= 0)
        {
            _hp = 0;
            _animator.SetTrigger("Death");

            MinotaurAI ai = GetComponent<MinotaurAI>();
            if (ai != null)
                ai.OnDeath();
        }
        else
        {
            _animator.SetTrigger("GetDamage");
        }
    }

    public void DeactivateMinotaurs()
    {
        gameObject.SetActive(false);
    }
}

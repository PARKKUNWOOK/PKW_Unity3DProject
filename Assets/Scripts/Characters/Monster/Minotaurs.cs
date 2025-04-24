using UnityEngine;

public class Minotaurs : MonoBehaviour
{
    private static Minotaurs _instance;
    public static Minotaurs Instance
    {
        get { return _instance; }
    }

    private Animator _animator;
    private int _hp = 10;

    private void Awake()
    {
        _instance = this;
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Debug.Log($"�̳�Ÿ�츣�� ���� ü��: {_hp}");
    }

    public void GetDamage(int damage)
    {
        _hp -= damage;
        Debug.Log($"�̳�Ÿ�츣�� �ǰ�! ���� HP: {_hp}");

        if (_hp <= 0)
        {
            _hp = 0;
            Debug.Log("�̳�Ÿ�츣�� ���");

            _animator.SetTrigger("Death");
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

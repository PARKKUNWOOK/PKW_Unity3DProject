using UnityEngine;

public class PlayerKnight : MonoBehaviour
{
    private static PlayerKnight _instance;
    public static PlayerKnight Instance
    {
        get { return _instance; }
    }

    private Animator _animator;
    private int _level;
    private int _maxLevel = 25;
    private int _hp;
    private int _mp;
    private int _attackPower;
    public int AttackPower { get { return _attackPower; } }

    private void Awake()
    {
        _instance = this;
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _level = 1;
        _hp = 10;
        _mp = 5;
        _attackPower = 1;
        Debug.Log($"�÷��̾� ���� �������ͽ�:\n����: {_level}\nHP: {_hp}\nMP: {_mp}\n���ݷ�: {_attackPower}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        if (_level >= _maxLevel)
        {
            Debug.Log($"�ְ���{_maxLevel} �޼�");
            return;
        }

        _level += 1;
        _hp += 50;
        _mp += 10;
        _attackPower += 3;
        Debug.Log($"�÷��̾� ���� �������ͽ�:\n����: {_level}\nHP: {_hp}\nMP: {_mp}\n���ݷ�: {_attackPower}");
    }

    public void GetDamage(int damage)
    {
        _hp -= damage;
        Debug.Log($"�÷��̾� �ǰ�! ���� HP: {_hp}");

        if (_hp <= 0)
        {
            _hp = 0;
            Debug.Log("�÷��̾� ���");

            _animator.SetTrigger("Death");
        }
        else
        {
            _animator.SetTrigger("GetDamage");
        }
    }

    public void DeactivatePlayer()
    {
        gameObject.SetActive(false);
    }
}

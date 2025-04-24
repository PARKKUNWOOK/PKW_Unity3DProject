using UnityEngine;

public class PlayerKnight : MonoBehaviour
{
    private static PlayerKnight _instance;
    public static PlayerKnight Instance
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
        Debug.Log($"�÷��̾� ���� ü��: {_hp}");
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

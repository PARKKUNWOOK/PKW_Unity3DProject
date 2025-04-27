using System.Collections.Generic;
using UnityEngine;

public class KnightSkill : MonoBehaviour
{
    private Animator _animator;
    private KnightAttack _attack;
    private KnightBlock _block;
    private PlayerSkillData _playerSkillData;

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

        HandleSkillInput(KeyCode.Alpha1, 101, "TripleSlashSkill");
        HandleSkillInput(KeyCode.Alpha2, 102, "JumpSkill");
        HandleSkillInput(KeyCode.Alpha3, 103, "PowerUpSkill");
        HandleSkillInput(KeyCode.Alpha4, 104, "SpinSlashSkill");
        HandleSkillInput(KeyCode.Alpha5, 105, "ChargeSkill");
    }

    private void HandleSkillInput(KeyCode keyCode, int skillKey, string animationTrigger)
    {
        if (Input.GetKeyDown(keyCode))
        {
            _playerSkillData = PlayerSkillDataManager.Instance.GetPlayerSkillData(skillKey);

            if (_playerSkillData.CurLevel <= 0)
            {
                Debug.Log($"{_playerSkillData.Name} ��ų ������ 0�̶� ����� �� �����ϴ�.");
                return;
            }

            if (PlayerKnight.Instance.CurMp >= _playerSkillData.MpCost)
            {
                PlayerKnight.Instance.ConsumeMp(_playerSkillData.MpCost);
                _animator.SetTrigger(animationTrigger);
                Debug.Log($"{_playerSkillData.Name} ��ų ���! MP {_playerSkillData.MpCost} �Ҹ�");
            }
            else
            {
                Debug.Log($"{_playerSkillData.Name} ��ų ��� �Ұ� - MP ����");
            }
        }
    }
}

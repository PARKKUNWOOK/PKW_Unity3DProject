using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Unity.Burst.Intrinsics.X86.Avx;

public class PlayerKnight : MonoBehaviour
{
    private static PlayerKnight _instance;
    public static PlayerKnight Instance { get { return _instance; } }

    private Animator _animator;
    private Image _hpBarImage;
    private Image _mpBarImage;
    private Image _expBarImage;
    private TextMeshProUGUI _hpBarText;
    private TextMeshProUGUI _mpBarText;
    private TextMeshProUGUI _expBarText;
    private TextMeshProUGUI _levelText;

    public int Level => _level;
    private int _level;
    private int _maxLevel = 25;
    private int _levelUpCount;
    private int _curHp;
    private int _maxHp;
    private int _curMp;
    private int _maxMp;
    private float _curExp;
    private float _maxExp;
    private int _attackPower;
    private bool _isPowerUpBuffActive = false;
    public bool IsPowerUpBuffActive => _isPowerUpBuffActive;
    private float _buffDuration = 0f;
    private float _buffTimer = 0f;
    private float _attackPowerBeforeBuff = 0f;

    public int AttackPower { get { return _attackPower; } }
    public int CurMp { get { return _curMp; } }

    private void Awake()
    {
        _instance = this;
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _hpBarImage = GameObject.Find("PlayerHpBar").GetComponent<Image>();
        _hpBarText = GameObject.Find("PlayerHpBarText").GetComponentInChildren<TextMeshProUGUI>();
        _mpBarImage = GameObject.Find("PlayerMpBar").GetComponent<Image>();
        _mpBarText = GameObject.Find("PlayerMpBarText").GetComponentInChildren<TextMeshProUGUI>();
        _expBarImage = GameObject.Find("PlayerExpBar").GetComponent<Image>();
        _expBarText = GameObject.Find("PlayerExpBarText").GetComponentInChildren<TextMeshProUGUI>();
        _levelText = GameObject.Find("LevelText").GetComponent<TextMeshProUGUI>();

        _level = 1;
        _maxHp = 10;
        _curHp = _maxHp;
        _maxMp = 5;
        _curMp = _maxMp;
        _maxExp = 100f;
        _curExp = 0f;
        _attackPower = 1;
        _levelUpCount = 0;

        UpdateHpBar();
        UpdateMpBar();
        UpdateExpBar();
        UpdateLevelText();

        Debug.Log($"�÷��̾� ���� ���ݷ�:\n���ݷ�: {_attackPower}");
    }

    private void Update()
    {
        //if (Input.GetKey(KeyCode.O))
        //{
        //    GainExp(50.95f);
        //}

        if (Input.GetKeyDown(KeyCode.O))
        {
            GainExp(50.95f);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            LevelUp();
        }

        HandleBuffTimer();
    }

    private void GainExp(float amount)
    {
        _curExp += amount;

        while (_curExp >= _maxExp)
        {
            _curExp -= _maxExp;
            LevelUp();
        }

        UpdateExpBar();
    }

    private void LevelUp()
    {
        if (_level >= _maxLevel)
        {
            Debug.Log($"�ְ���{_maxLevel} �޼�");
            _curExp = _maxExp;
            UpdateExpBar();
            return;
        }

        _level += 1;
        _levelUpCount += 1;
        _maxHp += 50;
        _curHp = _maxHp;
        _maxMp += 10;
        _curMp = _maxMp;
        _attackPower += 3;

        _maxExp += 40.5f * Mathf.Pow(2, _levelUpCount);

        if (SkillWindow.Instance != null)
        {
            SkillWindow.Instance.AddSkillPoint(3);
        }

        UpdateHpBar();
        UpdateMpBar();
        UpdateExpBar();
        UpdateLevelText();

        Debug.Log($"�÷��̾� ���� ���ݷ�:\n���ݷ�: {_attackPower}");
    }

    public void GetDamage(int damage)
    {
        _curHp -= damage;

        UpdateHpBar();

        if (_curHp <= 0)
        {
            _curHp = 0;

            _animator.SetTrigger("Death");
        }
        else
        {
            _animator.SetTrigger("GetDamage");
        }
    }

    public void ActivatePowerUpBuff(int attackPowerMultiplier, int duration)
    {
        if (_isPowerUpBuffActive)
            return;

        _isPowerUpBuffActive = true;
        _buffDuration = duration;
        _buffTimer = 0f;

        _attackPowerBeforeBuff = _attackPower;
        _attackPower *= attackPowerMultiplier;

        Debug.Log($"���� �ߵ�! ���ݷ� {attackPowerMultiplier}�� ���: {_attackPower}");
    }

    private void HandleBuffTimer()
    {
        if (!_isPowerUpBuffActive)
            return;

        _buffTimer += Time.deltaTime;

        if (_buffTimer >= _buffDuration)
        {
            _isPowerUpBuffActive = false;
            _attackPower = (int)_attackPowerBeforeBuff;

            Debug.Log($"���� ����! ���ݷ� ����: {_attackPower}");

            if (KnightSkill.CurrentSkillKey != -1 && KnightSkill.CurrentSkillKey != 103)
            {
                PlayerSkillData skillData = PlayerSkillDataManager.Instance.GetPlayerSkillData(KnightSkill.CurrentSkillKey);
                KnightSkill.CurrentSkillAttackPower = skillData.AttackPower;
                Debug.Log($"���� ����! ��ų ���ݷ� ����: {KnightSkill.CurrentSkillAttackPower}");
            }
        }
    }

    private void UpdateLevelText()
    {
        if (_levelText != null)
        {
            _levelText.text = $"LV: {_level}";
        }
    }

    private void UpdateHpBar()
    {
        if (_hpBarImage != null)
        {
            _hpBarImage.fillAmount = (float)_curHp / _maxHp;
        }

        if (_hpBarText != null)
        {
            _hpBarText.text = $"{_curHp} / {_maxHp}";
        }
    }

    private void UpdateMpBar()
    {
        if (_mpBarImage != null)
        {
            _mpBarImage.fillAmount = (float)_curMp / _maxMp;
        }

        if (_mpBarText != null)
        {
            _mpBarText.text = $"{_curMp} / {_maxMp}";
        }
    }

    private void UpdateExpBar()
    {
        if (_expBarImage != null)
            _expBarImage.fillAmount = _curExp / _maxExp;

        if (_expBarText != null)
        {
            float percent = (_curExp / _maxExp) * 100f;
            _expBarText.text = $"{percent:F1}%";
        }
    }

    public void ConsumeMp(int amount)
    {
        _curMp -= amount;
        if (_curMp < 0)
            _curMp = 0;

        UpdateMpBar();
    }

    public void DeactivatePlayer()
    {
        gameObject.SetActive(false);
    }
}

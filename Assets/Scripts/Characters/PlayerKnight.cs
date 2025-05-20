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
    private GameObject _buffIconObject;
    private Image _buffTimerImage;

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

    private float _buffFillDuration = 0f;
    private float _buffFillTimer = 0f;
    private float _attackPowerBeforeBuff = 0f;

    private bool _isBlinking = false;
    private float _blinkTimer = 0f;
    private float _blinkInterval = 0.3f;
    private bool _isVisible = true;
    private float _blinkSpeed = 5f;

    public int AttackPower { get { return _attackPower; } }
    public int CurHp { get { return _curHp; } }
    public int MaxHp { get { return _maxHp; } }
    public int CurMp { get { return _curMp; } }
    public int MaxMp { get { return _maxMp; } }
    private bool _isGettingHit = false;
    public bool IsGettingHit => _isGettingHit;
    public bool IsDead { get; private set; } = false;

    private bool _canControl = true;
    public bool CanControl => _canControl;

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

        _buffIconObject = GameObject.Find("BuffDescriptionTimeIcon");
        if (_buffIconObject != null)
        {
            Transform timerTransform = _buffIconObject.transform.Find("BuffDescriptionTime");
            if (timerTransform != null)
                _buffTimerImage = timerTransform.GetComponent<Image>();

            _buffIconObject.SetActive(false);
        }

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

        Debug.Log($"플레이어 시작 공격력:\n공격력: {_attackPower}");
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

    public void SetControlEnabled(bool value)
    {
        _canControl = value;

        if (_animator != null)
            _animator.enabled = value;
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
            Debug.Log($"최고레벨{_maxLevel} 달성");
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

        Debug.Log($"플레이어 현제 공격력:\n공격력: {_attackPower}");
    }

    public void GetDamage(int damage)
    {
        if (_isGettingHit) return;

        _curHp -= damage;
        UpdateHpBar();

        _isGettingHit = true;

        if (_curHp <= 0)
        {
            _curHp = 0;
            IsDead = true;
            _animator.SetTrigger("Death");
        }
        else
        {
            _animator.SetTrigger("GetDamage");
        }

        Vector3 hitPos = transform.position + Vector3.up * 2f;
        DamageTextManager.Instance.ShowDamage(damage, hitPos, true);
    }

    public void OnHitStart()
    {
        _isGettingHit = true;

        KnightAttack attack = GetComponent<KnightAttack>();
        if (attack != null)
            attack.ForceStopAttack();

        KnightSkill skill = GetComponent<KnightSkill>();
        if (skill != null)
        {
            skill.ForceStopSkill();
            skill.StopAllSkillEffects();
        }
            
    }

    public void OnHitEnd()
    {
        _isGettingHit = false;
    }

    public void HealHp(int amount)
    {
        _curHp = Mathf.Min(_curHp + amount, _maxHp);
        UpdateHpBar();
    }

    public void HealMp(int amount)
    {
        _curMp = Mathf.Min(_curMp + amount, _maxMp);
        UpdateMpBar();
    }

    public void ActivatePowerUpBuff(int attackPowerMultiplier, int duration)
    {
        if (_isPowerUpBuffActive)
        {
            _attackPower = (int)_attackPowerBeforeBuff;
        }

        _isPowerUpBuffActive = true;
        _buffDuration = duration;
        _buffTimer = 0f;

        _attackPowerBeforeBuff = _attackPower;
        _attackPower *= attackPowerMultiplier;

        if (_buffIconObject != null)
            _buffIconObject.SetActive(true);

        if (_buffTimerImage != null)
        {
            _buffFillDuration = duration;
            _buffFillTimer = 0f;
            _buffTimerImage.fillAmount = 1f;
        }

        _isBlinking = false;
        _blinkTimer = 0f;
        _isVisible = true;

        Debug.Log($"버프 발동! 공격력 {attackPowerMultiplier}배 상승, 일반공격력: {_attackPower}");
    }

    private void HandleBuffTimer()
    {
        if (!_isPowerUpBuffActive)
            return;

        _buffTimer += Time.deltaTime;
        _buffFillTimer += Time.deltaTime;

        if (_buffTimerImage != null && _buffFillDuration > 0)
        {
            float remaining = Mathf.Clamp01(1f - (_buffFillTimer / _buffFillDuration));
            _buffTimerImage.fillAmount = remaining;

            if (remaining <= 0.286f && !_isBlinking)
            {
                _isBlinking = true;
                _blinkTimer = 0f;
                _isVisible = true;
            }
        }

        if (_isBlinking && _buffIconObject != null)
        {
            CanvasGroup canvasGroup = _buffIconObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = _buffIconObject.AddComponent<CanvasGroup>();

            float alpha = 0.5f + 0.5f * Mathf.Sin(Time.time * _blinkSpeed);
            canvasGroup.alpha = alpha;
        }

        if (_buffTimer >= _buffDuration)
        {
            _isPowerUpBuffActive = false;
            _attackPower = (int)_attackPowerBeforeBuff;

            if (_buffIconObject != null)
            {
                _buffIconObject.SetActive(false);
                _isBlinking = false;

                CanvasGroup canvasGroup = _buffIconObject.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                    canvasGroup.alpha = 1f;
            }

            Debug.Log($"버프 종료! 공격력 복구: {_attackPower}");

            if (KnightSkill.CurrentSkillKey != -1 && KnightSkill.CurrentSkillKey != 103)
            {
                PlayerSkillData skillData = PlayerSkillDataManager.Instance.GetPlayerSkillData(KnightSkill.CurrentSkillKey);
                KnightSkill.CurrentSkillAttackPower = skillData.AttackPower;
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

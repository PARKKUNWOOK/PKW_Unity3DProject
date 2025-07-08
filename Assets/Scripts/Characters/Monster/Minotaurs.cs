using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Minotaurs : MonoBehaviour
{
    //Å×½ºÆ®
    private static int _globalIdCounter = 1;
    private int _minotaurId;

    /// ///////////////////////////////////////////
    
    private Animator _animator;
    private int _maxHp = 10;
    private int _curHp;

    public bool IsDead => _curHp <= 0;

    private GameObject _hpBarPrefab;
    private GameObject _hpBarObject;
    private Slider _hpSlider;

    private Image _hpFillImage;
    private Image _hpBLImage;
    private Image _hpBGImage;

    private Vector3 _hpBarOffset = new Vector3(0, 5.5f, 1);
    private float _hpBarVisibleTimer = 0f;
    private float _hpBarVisibleDuration = 3f;
    private bool _hpBarVisible = false;
    private bool _isFadingOut = false;
    private float _fadeSpeed = 5f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _curHp = _maxHp;

        _minotaurId = _globalIdCounter++;
        gameObject.name = $"Minotaurs_{_minotaurId}";

        _hpBarPrefab = Resources.Load<GameObject>("Prefabs/MonsterUI/MonsterHpBar");
    }

    private void Start()
    {
        GameObject hpBarPanel = GameObject.Find("MonsterHpBarPanel");
        _hpBarObject = Instantiate(_hpBarPrefab, hpBarPanel.transform);
        _hpSlider = _hpBarObject.GetComponent<Slider>();

        Transform imageRoot = _hpSlider.transform.Find("MonsterHpBarBG");
        _hpBGImage = imageRoot.GetComponent<Image>();
        _hpFillImage = imageRoot.Find("MonsterHp")?.GetComponent<Image>();
        _hpBLImage = imageRoot.Find("MonsterHpBarBL")?.GetComponent<Image>();

        _hpBarObject.SetActive(false);
        SetHpBarAlpha(0.5f);
    }

    private void Update()
    {
        if (_hpSlider != null && _hpBarObject.activeSelf)
        {
            Vector3 worldPosition = transform.position + _hpBarOffset;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

            if (screenPosition.z > 0)
            {
                _hpSlider.transform.position = screenPosition;
                _hpSlider.value = (float)_curHp / _maxHp;
            }
            else
            {
                _hpBarObject.SetActive(false);
            }
        }

        if (_hpBarVisible)
        {
            _hpBarVisibleTimer += Time.deltaTime;
            if (_hpBarVisibleTimer >= _hpBarVisibleDuration)
            {
                _hpBarVisible = false;
                _isFadingOut = true;
            }
        }

        if (_isFadingOut)
        {
            float newAlpha = Mathf.Lerp(_hpFillImage.color.a, 0, Time.deltaTime * _fadeSpeed);
            SetHpBarAlpha(newAlpha);

            if (newAlpha <= 0.05f)
            {
                _hpBarObject.SetActive(false);
                _isFadingOut = false;
            }
        }
    }

    public void GetDamage(int damage)
    {
        if (IsDead) return;

        _curHp -= damage;
        _curHp = Mathf.Max(0, _curHp);

        if (_hpSlider != null)
        {
            _hpBarObject.SetActive(true);
            SetHpBarAlpha(1f);
            _hpBarVisible = true;
            _hpBarVisibleTimer = 0f;
            _isFadingOut = false;
        }

        MinotaurAI ai = GetComponent<MinotaurAI>();
        if (ai != null)
            ai.OnHitStart();

        if (_curHp <= 0)
        {
            _curHp = 0;
            GetComponent<Collider>().enabled = false;
            _animator.SetTrigger("Death");

            if (ai != null)
                ai.OnDeath();

            _hpBarObject.SetActive(false);
            _hpBarVisible = false;

            PlayerKnight.Instance.SendMessage("GainExp", 50.95f);
            InventoryWindow.Instance?.AddGold(100);
            ItemIcon.Instance?.AddItem("HpPotion");
            ItemIcon.Instance?.AddItem("MpPotion");

            Dictionary<string, int> items = new Dictionary<string, int>
            {
                { "HpPotion", 1 },
                { "MpPotion", 1 }
            };

            GetTextPopupManager.Instance?.ShowExpGoldAndItems(50.95f, 100, items);

            MinotaursManager.Instance.RespawnMinotaur(
            gameObject,
            ai.GetSpawnPoint(),
            ai.PatrolPoints
            );
        }
        else
        {
            _animator.SetTrigger("GetDamage");
        }

        Vector3 hitPos = transform.position + Vector3.up * 2f;
        DamageTextManager.Instance.ShowDamage(damage, hitPos, false);
    }

    public void ResetMinotaur()
    {
        _curHp = _maxHp;
        _animator.Rebind();
        _animator.Update(0f);

        MinotaurEffectController effect = GetComponentInChildren<MinotaurEffectController>();
        if (effect != null)
            effect.ResetEffects();

        if (_hpBarObject != null)
        {
            _hpSlider.value = 1f;
            _hpBarObject.SetActive(false);
            SetHpBarAlpha(0.5f);
            _hpBarVisible = false;
            _hpBarVisibleTimer = 0f;
            _isFadingOut = false;
        }

    }

    private void SetHpBarAlpha(float alpha)
    {
        if (_hpBLImage != null)
        {
            Color c = _hpBLImage.color;
            c.a = alpha;
            _hpBLImage.color = c;
        }
        if (_hpFillImage != null)
        {
            Color c = _hpFillImage.color;
            c.a = alpha;
            _hpFillImage.color = c;
        }
        if (_hpBGImage != null)
        {
            Color c = _hpBGImage.color;
            c.a = alpha;
            _hpBGImage.color = c;
        }
    }

    public void DeactivateMinotaurs()
    {
        gameObject.SetActive(false);
    }
}

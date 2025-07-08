using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    public int AssignedSkillKey { get; private set; } = -1;

    private Image _coolTimeImage;
    private float _coolTimeDuration;
    private float _coolTimeRemaining;
    private bool _isCoolingDown = false;

    private void Awake()
    {
        //string slotNumber = gameObject.name.Replace("SkillSlot", "");
        //Transform coolTimeTransform = transform.Find($"SkillCoolTime{slotNumber}");
        //
        //_coolTimeImage = coolTimeTransform.GetComponent<Image>();
        //_coolTimeImage.gameObject.SetActive(false);

        foreach (Transform child in transform)
        {
            if (child.CompareTag("SkillCoolTime"))
            {
                _coolTimeImage = child.GetComponent<Image>();
                _coolTimeImage.gameObject.SetActive(false);
                return;
            }
        }
    }

    private void Update()
    {
        if (_isCoolingDown)
        {
            _coolTimeRemaining -= Time.deltaTime;
            _coolTimeImage.fillAmount = _coolTimeRemaining / _coolTimeDuration;

            if (_coolTimeRemaining <= 0f)
            {
                _isCoolingDown = false;
                _coolTimeImage.gameObject.SetActive(false);
            }
        }
    }

    public void AssignSkill(int skillKey)
    {
        AssignedSkillKey = skillKey;
    }

    public void ClearSlot()
    {
        AssignedSkillKey = -1;
    }

    public bool IsCoolingDown()
    {
        return _isCoolingDown;
    }

    public void StartCoolTime(float duration)
    {
        _coolTimeDuration = duration;
        _coolTimeRemaining = duration;
        _isCoolingDown = true;
        _coolTimeImage.fillAmount = 1f;
        _coolTimeImage.gameObject.SetActive(true);
    }
}

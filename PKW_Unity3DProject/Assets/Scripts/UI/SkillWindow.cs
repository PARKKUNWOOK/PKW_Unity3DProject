using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillWindow : MonoBehaviour
{
    public static SkillWindow Instance;
    public int CurrentSkillPoint => skillPoint;

    public GameObject skillWindowUI;
    public Button closeButton;
    public TextMeshProUGUI skillPointText;

    private int skillPoint = 0;
    private bool isActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        skillWindowUI.SetActive(false);
        closeButton.onClick.AddListener(ToggleSkillWindow);
        UpdateSkillPointText();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ToggleSkillWindow();
        }
    }

    public void AddSkillPoint(int amount)
    {
        skillPoint += amount;
        UpdateSkillPointText();
    }

    public bool SkillLevelUp(int amount)
    {
        if (skillPoint >= amount)
        {
            skillPoint -= amount;
            UpdateSkillPointText();
            return true;
        }
        Debug.Log("스킬 포인트가 부족합니다!");
        return false;
    }

    private void UpdateSkillPointText()
    {
        if (skillPointText != null)
        {
            skillPointText.text = skillPoint.ToString();
        }
    }

    void ToggleSkillWindow()
    {
        isActive = !isActive;
        skillWindowUI.SetActive(isActive);
    }
}

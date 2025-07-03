using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWindow : MonoBehaviour
{
    public static InventoryWindow Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI goldText;
    private int _gold = 0;
    public int Gold => _gold;

    [SerializeField] private GameObject inventoryWindowObject;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateGoldText();

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseInventory);

        if (inventoryWindowObject != null)
            inventoryWindowObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        if (inventoryWindowObject != null)
        {
            bool isActive = inventoryWindowObject.activeSelf;
            inventoryWindowObject.SetActive(!isActive);
        }
    }

    private void CloseInventory()
    {
        if (inventoryWindowObject != null)
        {
            inventoryWindowObject.SetActive(false);
        }
    }

    public void AddGold(int amount)
    {
        _gold += amount;
        UpdateGoldText();
    }

    private void UpdateGoldText()
    {
        if (goldText != null)
        {
            goldText.text = _gold.ToString();
        }
    }
}

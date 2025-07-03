using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance;

    [SerializeField] private GameObject damageTextPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowDamage(int damage, Vector3 worldPos, bool isPlayerDamaged)
    {
        GameObject textObj = Instantiate(damageTextPrefab, this.transform);
        DamageTextController controller = textObj.GetComponent<DamageTextController>();

        Color color = isPlayerDamaged ? Color.red : Color.white;
        controller.Initialize(damage, worldPos, color);
    }
}

using UnityEngine;

public class HitEffectController : MonoBehaviour
{
    public static HitEffectController Instance { get; private set; }

    [SerializeField] private GameObject hitEffectPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayHitEffect(Vector3 position)
    {
        if (hitEffectPrefab == null) return;

        GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(effect, 1.5f);
    }
}

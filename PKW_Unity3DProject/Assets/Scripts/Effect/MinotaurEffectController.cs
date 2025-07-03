using UnityEngine;

public class MinotaurEffectController : MonoBehaviour
{
    [SerializeField] private GameObject attackEffect;

    private void Start()
    {
        if (attackEffect != null)
            attackEffect.SetActive(false);
    }

    public void PlayAttackEffect()
    {
        if (!attackEffect) return;

        attackEffect.SetActive(false);
        attackEffect.SetActive(true);
    }

    public void ResetEffects()
    {
        if (attackEffect != null)
            attackEffect.SetActive(false);
    }

}

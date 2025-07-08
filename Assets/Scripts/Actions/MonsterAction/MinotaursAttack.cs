using UnityEngine;

public class MinotaursAttack : MonoBehaviour
{
    [SerializeField]
    private BoxCollider _axeCollider;

    private void Start()
    {
        if (_axeCollider != null)
            _axeCollider.enabled = false;
    }

    public void EnableAxeCollider()
    {
        if (_axeCollider != null)
            _axeCollider.enabled = true;
    }

    public void DisableAxeCollider()
    {
        if (_axeCollider != null)
            _axeCollider.enabled = false;
    }
}

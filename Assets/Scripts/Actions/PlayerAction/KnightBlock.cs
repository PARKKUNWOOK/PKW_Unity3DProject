using UnityEngine;

public class KnightBlock : MonoBehaviour
{
    private Animator _animator;

    [SerializeField]
    private Transform _shieldTransform;
    [SerializeField]
    private BoxCollider _shieldCollider;

    public bool IsBlocking { get; private set; }

    private Vector3 originalRotation;
    private bool isRotationSet = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_shieldTransform != null)
            originalRotation = _shieldTransform.localEulerAngles;
        if (_shieldCollider != null)
            _shieldCollider.enabled = false;
    }

    private void Update()
    {
        if (!PlayerKnight.Instance.CanControl) return;
        if (PlayerKnight.Instance.IsDead)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartBlocking();
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            StopBlocking();
            if (_shieldCollider != null)
                _shieldCollider.enabled = false;
        }

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        IsBlocking = stateInfo.IsTag("Blocking");
    }

    private void StartBlocking()
    {
        _animator.SetBool("isBlocking", true);
    }

    private void StopBlocking()
    {
        _animator.SetBool("isBlocking", false);

        if (_shieldTransform != null && isRotationSet)
        {
            _shieldTransform.localEulerAngles = originalRotation;
            isRotationSet = false;
        }
    }

    public void SetShieldRotation()
    {
        if (_shieldTransform == null)
            return;

        Vector3 currentRotation = _shieldTransform.localEulerAngles;
        _shieldTransform.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, 113.26f);

        isRotationSet = true;
    }

    public void EnableShieldCollider()
    {
        if (_shieldCollider != null)
            _shieldCollider.enabled = true;
    }
}

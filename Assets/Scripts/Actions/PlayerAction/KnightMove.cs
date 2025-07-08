using UnityEngine;

public class KnightMove : MonoBehaviour
{
    private Animator _animator;
    private KnightAttack _attack;
    private KnightBlock _block;
    private KnightSkill _skill;

    private float _walkSpeed = 3f;
    private float _runSpeed = 6f;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _attack = GetComponent<KnightAttack>();
        _block = GetComponent<KnightBlock>();
        _skill = GetComponent<KnightSkill>();
    }

    void Update()
    {
        if (!PlayerKnight.Instance.CanControl) return;
        if (PlayerKnight.Instance.IsGettingHit) return;

        if ((_attack != null && _attack.IsAttacking) || (_block != null && _block.IsBlocking) || (_skill != null && _skill.IsSkill) || PlayerKnight.Instance.IsDead)
        {
            if (_animator != null)
                _animator.SetFloat("Speed", 0f);
            return;
        }

        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = new Vector3(horizontal, 0f, vertical).normalized;

        bool isMoving = moveDir.magnitude > 0f;
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);

        float targetSpeed = 0f;

        if (isMoving)
        {
            transform.forward = moveDir;

            targetSpeed = isRunning ? 6f : 3f;
            transform.position += moveDir * targetSpeed * Time.deltaTime;
        }

        if (_animator != null)
        {
            _animator.SetFloat("Speed", targetSpeed);
        }
    }
}

using UnityEngine;
using UnityEngine.AI;

public enum MinotaurState
{
    Patrol,
    Chase,
    Attack,
    Return
}

public class MinotaurAI : MonoBehaviour
{
    public float chaseRange = 10f;
    public float attackRange = 3f;
    public float returnRange = 15f;
    public Transform[] PatrolPoints { get; set; }

    private Transform _player;
    private NavMeshAgent _agent;
    private Animator _animator;
    private Vector3 _spawnPoint;
    private int _patrolIndex = 0;
    
    private MinotaurState _currentState = MinotaurState.Patrol;
    private bool _isAttacking = false;
    private bool _isGettingHit = false;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _player = GameManager.Instance.Player.transform;
        _spawnPoint = transform.position;

        GoToNextPatrol();
    }

    private void Update()
    {
        if (GetComponent<Minotaurs>().IsDead) return;
        if (_isGettingHit) return;

        float distance = Vector3.Distance(transform.position, _player.position);

        switch (_currentState)
        {
            case MinotaurState.Patrol:
                if (distance < chaseRange)
                    _currentState = MinotaurState.Chase;
                Patrol();
                break;

            case MinotaurState.Chase:
                if (distance < attackRange)
                    _currentState = MinotaurState.Attack;
                else if (distance > returnRange)
                    _currentState = MinotaurState.Return;
                else
                    Chase();
                break;

            case MinotaurState.Attack:
                if (distance > attackRange)
                    _currentState = MinotaurState.Chase;
                else
                    Attack();
                break;

            case MinotaurState.Return:
                if (distance < chaseRange)
                {
                    _currentState = MinotaurState.Chase;
                }
                else if (Vector3.Distance(transform.position, _spawnPoint) < 0.5f)
                {
                    _currentState = MinotaurState.Patrol;
                    GoToNextPatrol();
                }
                else
                {
                    ReturnToSpawn();
                }
                break;
        }
    }

    private void Patrol()
    {
        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
            GoToNextPatrol();

        _animator.SetFloat("Speed", _agent.velocity.magnitude);
    }

    private void GoToNextPatrol()
    {
        if (PatrolPoints.Length == 0) return;
        if (_agent == null || !_agent.isOnNavMesh)
        {
            return;
        }

        _agent.SetDestination(PatrolPoints[_patrolIndex].position);
        _patrolIndex = (_patrolIndex + 1) % PatrolPoints.Length;
    }

    private void Chase()
    {
        _agent.SetDestination(_player.position);
        _animator.SetFloat("Speed", _agent.velocity.magnitude);
    }

    private void Attack()
    {
        if (_isAttacking) return;

        _agent.SetDestination(transform.position);
        transform.LookAt(_player);
        _animator.SetFloat("Speed", 0f);

        _isAttacking = true;
        _animator.SetTrigger("Attack");
    }

    public void OnAttackStart()
    {
        _agent.isStopped = true;
    }

    private void ReturnToSpawn()
    {
        _agent.SetDestination(_spawnPoint);
        _animator.SetFloat("Speed", _agent.velocity.magnitude);
    }

    public Vector3 GetSpawnPoint()
    {
        return _spawnPoint;
    }

    public void OnHitStart()
    {
        _isGettingHit = true;
        _agent.isStopped = true;
    }

    public void OnHitEnd()
    {
        _isGettingHit = false;
        _agent.isStopped = false;
        _isAttacking = false;
    }

    public void OnDeath()
    {
        _agent.isStopped = true;
        this.enabled = false;

        Collider bodyCollider = GetComponent<Collider>();
        if (bodyCollider != null)
            bodyCollider.enabled = false;

        MeleeWeaponTrigger weapon = GetComponentInChildren<MeleeWeaponTrigger>();
        if (weapon != null)
        {
            Collider weaponCol = weapon.GetComponent<Collider>();
            if (weaponCol != null)
                weaponCol.enabled = false;
        }
    }

    public void EndAttack()
    {
        _agent.isStopped = false;
        _isAttacking = false;
    }
    public void Initialize()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _player = GameManager.Instance.Player.transform;
        _spawnPoint = transform.position;
        _currentState = MinotaurState.Patrol;
        _isAttacking = false;
        _isGettingHit = false;
        GoToNextPatrol();

        Collider bodyCollider = GetComponent<Collider>();
        if (bodyCollider != null)
            bodyCollider.enabled = true;
    }

    public void SpawnAttackEffect()
    {
        MinotaurEffectController effect = GetComponentInChildren<MinotaurEffectController>();
        if (effect != null)
            effect.PlayAttackEffect();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, returnRange);
    }
}

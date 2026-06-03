using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(TargetHealthNet))]
[RequireComponent(typeof(TankShootingNet))]
public abstract class EnemyAINet : NetworkBehaviour
{
    [SerializeField] protected float _fireRate = 1.5f;
    [SerializeField] protected float _rotateSpeed = 120f;
    [SerializeField] protected float _moveSpeed = 3.5f;

    [Header("Target Search")]
    [SerializeField] private float _targetSearchInterval = 0.5f;

    protected NavMeshAgent _agent;
    protected TankShootingNet _shooting;
    protected float _fireTimer;
    protected bool _isAiming;

    protected Transform _flagTarget;
    protected Collider2D _flagCollider;

    private float _targetSearchTimer;

    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _shooting = GetComponent<TankShootingNet>();

        _shooting.IsComputerControlled = true;

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.speed = _moveSpeed;
    }

    protected virtual void Update()
    {
        if (!IsServerInitialized)
        {
            return;
        }

        _fireTimer += Time.deltaTime;
        UpdateFlagTarget();
        OnUpdateAI();

        if (!_isAiming)
        {
            UpdateRotationByVelocity();
        }
    }

    protected abstract void OnUpdateAI();

    protected void UpdateFlagTarget()
    {
        if (_flagTarget != null && _flagTarget.gameObject.activeInHierarchy)
        {
            return;
        }

        _targetSearchTimer -= Time.deltaTime;

        if (_targetSearchTimer > 0f)
        {
            return;
        }

        _targetSearchTimer = _targetSearchInterval;

        if (GameManagerNet.Instance == null || GameManagerNet.Instance.WorldObjectsSpawner == null)
        {
            return;
        }

        TargetHealthNet flagHealth = GameManagerNet.Instance.WorldObjectsSpawner.FlagHealth;

        if (flagHealth == null)
        {
            return;
        }

        _flagTarget = flagHealth.transform;
        _flagCollider = flagHealth.GetComponent<Collider2D>();
    }

    protected Transform GetNearestPlayer()
    {
        Transform nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (NetworkConnection connection in InstanceFinder.ServerManager.Clients.Values)
        {
            if (connection == null || connection.FirstObject == null)
            {
                continue;
            }

            Transform playerTransform = connection.FirstObject.transform;

            if (!playerTransform.gameObject.activeInHierarchy)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, playerTransform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = playerTransform;
            }
        }

        return nearest;
    }

    protected Transform GetNearestTurret()
    {
        if (GameManagerNet.Instance == null || GameManagerNet.Instance.WorldObjectsSpawner == null)
        {
            return null;
        }

        var turrets = GameManagerNet.Instance.WorldObjectsSpawner.Turrets;

        if (turrets == null || turrets.Count == 0)
        {
            return null;
        }

        Transform nearest = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < turrets.Count; i++)
        {
            AutoTurretNet turret = turrets[i];

            if (turret == null || !turret.gameObject.activeInHierarchy)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, turret.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = turret.transform;
            }
        }

        return nearest;
    }

    protected Transform GetNearestDefenseTarget()
    {
        Transform turretTarget = GetNearestTurret();

        if (_flagTarget == null || !_flagTarget.gameObject.activeInHierarchy)
        {
            return turretTarget;
        }

        if (turretTarget == null)
        {
            return _flagTarget;
        }

        float flagDistance = Vector2.Distance(transform.position, _flagTarget.position);
        float turretDistance = Vector2.Distance(transform.position, turretTarget.position);

        return flagDistance <= turretDistance ? _flagTarget : turretTarget;
    }

    protected Transform GetCloserTarget(Transform firstTarget, Transform secondTarget)
    {
        if (firstTarget == null)
        {
            return secondTarget;
        }

        if (secondTarget == null)
        {
            return firstTarget;
        }

        float firstDistance = Vector2.Distance(transform.position, firstTarget.position);
        float secondDistance = Vector2.Distance(transform.position, secondTarget.position);

        return firstDistance <= secondDistance ? firstTarget : secondTarget;
    }

    protected Vector2 GetTargetCenter(Transform target)
    {
        if (target == null)
        {
            return transform.position;
        }

        Collider2D targetCollider = target.GetComponent<Collider2D>();

        if (targetCollider != null)
        {
            return targetCollider.bounds.center;
        }

        return target.position;
    }

    protected bool RotateToward(Vector3 target, float speed)
    {
        Vector2 direction = ((Vector2)target - (Vector2)transform.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        float currentAngle = transform.eulerAngles.z;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, speed * Time.deltaTime);

        transform.rotation = Quaternion.AngleAxis(newAngle, Vector3.forward);
        return Mathf.Abs(Mathf.DeltaAngle(newAngle, targetAngle)) < 2f;
    }

    protected void UpdateRotationByVelocity()
    {
        if (_agent.velocity.sqrMagnitude < 0.01f)
        {
            return;
        }

        float angle = Mathf.Atan2(_agent.velocity.y, _agent.velocity.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    protected void MoveTo(Vector3 target)
    {
        if (!_agent.isOnNavMesh)
        {
            return;
        }

        _agent.isStopped = false;
        _agent.SetDestination(target);
    }

    protected void StopMoving()
    {
        if (!_agent.isOnNavMesh)
        {
            return;
        }

        _agent.isStopped = true;
        _agent.ResetPath();
    }

    protected void TryShoot()
    {
        if (_fireTimer < _fireRate)
        {
            return;
        }

        _fireTimer = 0f;
        _shooting.FireFromAI();
    }

    protected bool IsFacing(Vector2 target, float toleranceDeg)
    {
        Vector2 directionToTarget = (target - (Vector2)transform.position).normalized;
        float angle = Vector2.Angle((Vector2)transform.up, directionToTarget);
        return angle < toleranceDeg;
    }

    protected bool HasClearShot(Vector2 target, LayerMask obstacleMask)
    {
        Vector2 origin = transform.position;
        Vector2 direction = (target - origin).normalized;
        float distance = Vector2.Distance(origin, target);
        Vector2 rayStart = origin + direction * 0.6f;
        float rayDistance = distance - 0.6f - 0.5f;

        if (rayDistance <= 0f)
        {
            return true;
        }

        RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, rayDistance, obstacleMask);
        Debug.DrawRay(rayStart, direction * rayDistance, hit.collider == null ? Color.green : Color.red);

        return hit.collider == null;
    }
}
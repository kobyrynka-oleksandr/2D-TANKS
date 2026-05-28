using FishNet.Managing.Server;
using FishNet.Object;
using UnityEngine;

public class AutoTurretNet : NetworkBehaviour
{
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private NetworkObject _projectilePrefab;
    [SerializeField] private AudioSource _shootingAudio;

    [SerializeField] private float _detectionRadius = 8f;
    [SerializeField] private float _rotationSpeed = 180f;
    [SerializeField] private float _shotCooldown = 0.75f;
    [SerializeField] private float _projectileSpeed = 10f;
    [SerializeField] private float _maxDamage = 100f;
    [SerializeField] private float _explosionRadius = 1.5f;
    [SerializeField] private float _targetSearchInterval = 0.2f;

    [SerializeField] private LayerMask _enemyLayerMask;
    [SerializeField] private LayerMask _obstacleLayerMask;

    private Transform _currentTarget;
    private float _nextShotTime;
    private float _nextTargetSearchTime;

    private void Update()
    {
        if (!IsServerStarted)
        {
            return;
        }

        UpdateTargetByTimer();

        if (_currentTarget == null)
        {
            return;
        }

        RotateHeadToTarget();
        TryShootAtTarget();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }

    private void UpdateTargetByTimer()
    {
        if (Time.time < _nextTargetSearchTime)
        {
            return;
        }

        _nextTargetSearchTime = Time.time + _targetSearchInterval;
        _currentTarget = FindClosestVisibleTarget();
    }

    private Transform FindClosestVisibleTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            _detectionRadius,
            _enemyLayerMask);

        Transform closestTarget = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            Transform target = hits[i].transform;

            if (!IsTargetValid(target))
            {
                continue;
            }

            if (!HasLineOfSight(target))
            {
                continue;
            }

            float distance = GetDistanceToTarget(target);

            if (distance >= closestDistance)
            {
                continue;
            }

            closestDistance = distance;
            closestTarget = target;
        }

        return closestTarget;
    }

    private bool IsTargetValid(Transform target)
    {
        if (target == null)
        {
            return false;
        }

        if (!target.gameObject.activeInHierarchy)
        {
            return false;
        }

        return true;
    }

    private float GetDistanceToTarget(Transform target)
    {
        return Vector2.Distance(transform.position, target.position);
    }

    private void RotateHeadToTarget()
    {
        Vector2 direction = GetDirectionToTarget(_currentTarget);
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

        _head.rotation = Quaternion.RotateTowards(
            _head.rotation,
            targetRotation,
            _rotationSpeed * Time.deltaTime);
    }

    private void TryShootAtTarget()
    {
        if (Time.time < _nextShotTime)
        {
            return;
        }

        if (!HasLineOfSight(_currentTarget))
        {
            return;
        }

        SpawnProjectile();
        _nextShotTime = Time.time + _shotCooldown;
        RpcPlayShootSound();
    }

    private void SpawnProjectile()
    {
        NetworkObject projectileInstance = Instantiate(
            _projectilePrefab,
            _firePoint.position,
            _firePoint.rotation);

        InitializeProjectile(projectileInstance);
        ServerManager.Spawn(projectileInstance.gameObject);
    }

    private void InitializeProjectile(NetworkObject projectileInstance)
    {
        Vector2 direction = _firePoint.up;

        ShellProjectileNet shellProjectile = projectileInstance.GetComponent<ShellProjectileNet>();
        ShellExplosion2DNet shellExplosion = projectileInstance.GetComponent<ShellExplosion2DNet>();

        if (shellProjectile != null)
        {
            shellProjectile.Initialize(direction, _projectileSpeed);
        }

        if (shellExplosion != null)
        {
            shellExplosion.m_MaxDamage = _maxDamage;
            shellExplosion.m_ExplosionRadius = _explosionRadius;
            shellExplosion.m_Shooter = gameObject;
        }
    }

    private Vector2 GetDirectionToTarget(Transform target)
    {
        Vector2 direction = target.position - _head.position;
        return direction.normalized;
    }

    private bool HasLineOfSight(Transform target)
    {
        Vector2 origin = _firePoint.position;
        Vector2 direction = target.position - _firePoint.position;
        float distance = direction.magnitude;

        int layerMask = _enemyLayerMask | _obstacleLayerMask;
        RaycastHit2D hit = Physics2D.Raycast(origin, direction.normalized, distance, layerMask);

        if (hit.collider == null)
        {
            return false;
        }

        return hit.transform == target;
    }

    [ObserversRpc]
    private void RpcPlayShootSound()
    {
        if (_shootingAudio == null)
        {
            return;
        }

        _shootingAudio.Play();
    }
}

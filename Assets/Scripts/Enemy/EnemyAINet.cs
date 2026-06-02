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
    [SerializeField] protected float m_FireRate = 1.5f;
    [SerializeField] protected float m_RotateSpeed = 120f;
    [SerializeField] protected float m_MoveSpeed = 3.5f;

    [Header("Target Search")]
    [SerializeField] private float m_TargetSearchInterval = 0.5f;

    protected NavMeshAgent m_Agent;
    protected TankShootingNet m_Shooting;
    protected float m_FireTimer;
    protected bool m_IsAiming;

    protected Transform m_FlagTarget;
    protected Collider2D m_FlagCollider;

    private float m_TargetSearchTimer;

    protected virtual void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Shooting = GetComponent<TankShootingNet>();

        m_Shooting.m_IsComputerControlled = true;

        m_Agent.updateRotation = false;
        m_Agent.updateUpAxis = false;
        m_Agent.speed = m_MoveSpeed;
    }

    protected virtual void Update()
    {
        if (!IsServerInitialized)
        {
            return;
        }

        m_FireTimer += Time.deltaTime;
        UpdateFlagTarget();
        OnUpdateAI();

        if (!m_IsAiming)
        {
            UpdateRotationByVelocity();
        }
    }

    protected abstract void OnUpdateAI();

    protected void UpdateFlagTarget()
    {
        if (m_FlagTarget != null && m_FlagTarget.gameObject.activeInHierarchy)
        {
            return;
        }

        m_TargetSearchTimer -= Time.deltaTime;

        if (m_TargetSearchTimer > 0f)
        {
            return;
        }

        m_TargetSearchTimer = m_TargetSearchInterval;

        if (GameManagerNet.Instance == null || GameManagerNet.Instance.WorldObjectsSpawner == null)
        {
            return;
        }

        TargetHealthNet flagHealth = GameManagerNet.Instance.WorldObjectsSpawner.FlagHealth;

        if (flagHealth == null)
        {
            return;
        }

        m_FlagTarget = flagHealth.transform;
        m_FlagCollider = flagHealth.GetComponent<Collider2D>();
    }

    protected Transform GetNearestPlayer()
    {
        Transform nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (NetworkConnection conn in InstanceFinder.ServerManager.Clients.Values)
        {
            if (conn == null || conn.FirstObject == null)
            {
                continue;
            }

            Transform playerTransform = conn.FirstObject.transform;

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

        if (m_FlagTarget == null || !m_FlagTarget.gameObject.activeInHierarchy)
        {
            return turretTarget;
        }

        if (turretTarget == null)
        {
            return m_FlagTarget;
        }

        float flagDistance = Vector2.Distance(transform.position, m_FlagTarget.position);
        float turretDistance = Vector2.Distance(transform.position, turretTarget.position);

        return flagDistance <= turretDistance ? m_FlagTarget : turretTarget;
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
        Vector2 dir = ((Vector2)target - (Vector2)transform.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        float currentAngle = transform.eulerAngles.z;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, speed * Time.deltaTime);

        transform.rotation = Quaternion.AngleAxis(newAngle, Vector3.forward);
        return Mathf.Abs(Mathf.DeltaAngle(newAngle, targetAngle)) < 2f;
    }

    protected void UpdateRotationByVelocity()
    {
        if (m_Agent.velocity.sqrMagnitude < 0.01f)
        {
            return;
        }

        float angle = Mathf.Atan2(m_Agent.velocity.y, m_Agent.velocity.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    protected void MoveTo(Vector3 target)
    {
        if (!m_Agent.isOnNavMesh)
        {
            return;
        }

        m_Agent.isStopped = false;
        m_Agent.SetDestination(target);
    }

    protected void StopMoving()
    {
        if (!m_Agent.isOnNavMesh)
        {
            return;
        }

        m_Agent.isStopped = true;
        m_Agent.ResetPath();
    }

    protected void TryShoot()
    {
        if (m_FireTimer < m_FireRate)
        {
            return;
        }

        m_FireTimer = 0f;
        m_Shooting.FireFromAI();
    }

    protected bool IsFacing(Vector2 target, float toleranceDeg)
    {
        Vector2 dirToTarget = (target - (Vector2)transform.position).normalized;
        float angle = Vector2.Angle((Vector2)transform.up, dirToTarget);
        return angle < toleranceDeg;
    }

    protected bool HasClearShot(Vector2 target, LayerMask obstacleMask)
    {
        Vector2 origin = transform.position;
        Vector2 direction = (target - origin).normalized;
        float distance = Vector2.Distance(origin, target);
        Vector2 rayStart = origin + direction * 0.6f;
        float rayDist = distance - 0.6f - 0.5f;

        if (rayDist <= 0f)
        {
            return true;
        }

        RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, rayDist, obstacleMask);
        Debug.DrawRay(rayStart, direction * rayDist, hit.collider == null ? Color.green : Color.red);

        return hit.collider == null;
    }
}
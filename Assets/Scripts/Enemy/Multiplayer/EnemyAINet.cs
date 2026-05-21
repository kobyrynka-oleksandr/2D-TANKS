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
    [SerializeField] private float m_FlagSearchInterval = 0.5f;

    protected NavMeshAgent m_Agent;
    protected TankShootingNet m_Shooting;
    protected float m_FireTimer;
    protected bool m_IsAiming;

    protected Transform m_FlagTarget;
    protected Collider2D m_FlagCollider;

    private float m_FlagSearchTimer;

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

        m_FlagSearchTimer -= Time.deltaTime;
        if (m_FlagSearchTimer > 0f)
        {
            return;
        }

        m_FlagSearchTimer = m_FlagSearchInterval;

        GameObject flag = GameObject.FindWithTag("Flag");
        if (flag != null)
        {
            m_FlagTarget = flag.transform;
            m_FlagCollider = flag.GetComponent<Collider2D>();
        }
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
}
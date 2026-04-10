using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(TargetHealth))]
[RequireComponent(typeof(PlayerShooting))]
public abstract class EnemyAI : MonoBehaviour
{
    [SerializeField] protected float m_FireRate = 1.5f;
    [SerializeField] protected float m_RotateSpeed = 120f;
    [SerializeField] protected float m_MoveSpeed = 3.5f;

    protected NavMeshAgent m_Agent;
    protected PlayerShooting m_Shooting;
    protected float m_FireTimer;
    protected bool m_IsAiming = false;

    protected virtual void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Shooting = GetComponent<PlayerShooting>();
        m_Shooting.m_IsComputerControlled = true;

        m_Agent.updateRotation = false;
        m_Agent.updateUpAxis = false;
        m_Agent.speed = m_MoveSpeed;
    }

    protected virtual void Update()
    {
        m_FireTimer += Time.deltaTime;
        OnUpdateAI();

        if (!m_IsAiming)
            UpdateRotationByVelocity();
    }

    protected abstract void OnUpdateAI();

    protected bool RotateToward(Vector3 target, float speed)
    {
        Vector2 dir = ((Vector2)target - (Vector2)transform.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        float currentAngle = transform.eulerAngles.z;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, speed * Time.deltaTime);

        transform.rotation = Quaternion.AngleAxis(newAngle, Vector3.forward);

        return Mathf.Abs(Mathf.DeltaAngle(newAngle, targetAngle)) < 2f;
    }

    private void UpdateRotationByVelocity()
    {
        if (m_Agent.velocity.sqrMagnitude < 0.01f) return;

        float angle = Mathf.Atan2(m_Agent.velocity.y, m_Agent.velocity.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    protected void MoveTo(Vector3 target)
    {
        m_Agent.SetDestination(target);
    }

    protected void StopMoving()
    {
        m_Agent.SetDestination(transform.position);
    }

    protected void TryShoot()
    {
        if (m_FireTimer < m_FireRate) return;
        m_FireTimer = 0f;
        m_Shooting.Fire();
    }
}
using UnityEngine;

public class DestroyerAI : EnemyAI
{
    [SerializeField] private float m_ShootAtFlagRange = 8f;
    [SerializeField] private float m_StopDistance = 6f;
    [SerializeField] private LayerMask m_ObstacleMask;

    private Transform m_FlagTarget;
    private Vector2 m_FlagCenter;
    private Collider2D m_FlagCollider;

    protected override void Awake()
    {
        base.Awake();

        GameObject flag = GameObject.FindWithTag("Flag");
        if (flag)
        {
            m_FlagTarget = flag.transform;
            m_FlagCollider = flag.GetComponent<Collider2D>();
        }
    }

    protected override void OnUpdateAI()
    {
        if (!m_FlagTarget || !m_FlagTarget.gameObject.activeInHierarchy) return;

        m_FlagCenter = m_FlagCollider
            ? (Vector2)m_FlagCollider.bounds.center
            : (Vector2)m_FlagTarget.position;

        float distToFlag = Vector2.Distance(transform.position, m_FlagCenter);

        if (distToFlag > m_StopDistance)
        {
            m_IsAiming = false;
            MoveTo(m_FlagTarget.position);
        }
        else
        {
            StopMoving();
        }

        if (distToFlag <= m_ShootAtFlagRange)
        {
            m_IsAiming = true;
            RotateToward(m_FlagCenter, m_RotateSpeed);

            if (IsFacing(m_FlagCenter, 15f) && HasClearShot(m_FlagCenter))
                TryShoot();
        }
    }

    private bool IsFacing(Vector2 target, float toleranceDeg)
    {
        Vector2 dirToTarget = (target - (Vector2)transform.position).normalized;
        float angle = Vector2.Angle((Vector2)transform.up, dirToTarget);
        return angle < toleranceDeg;
    }

    private bool HasClearShot(Vector2 target)
    {
        Vector2 origin = (Vector2)transform.position;
        Vector2 direction = (target - origin).normalized;
        float distance = Vector2.Distance(origin, target);
        Vector2 rayStart = origin + direction * 0.6f;
        float rayDist = distance - 0.6f - 0.5f;

        if (rayDist <= 0) return true;

        RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, rayDist, m_ObstacleMask);
        Debug.DrawRay(rayStart, direction * rayDist, hit.collider == null ? Color.green : Color.red);

        return hit.collider == null;
    }
}
using UnityEngine;

public class KillerAI : EnemyAI
{
    [SerializeField] private float m_StopDistance = 4f;
    [SerializeField] private float m_ShootRange = 6f;
    [SerializeField] private LayerMask m_ObstacleMask;

    private Transform m_Player;
    private Collider2D m_PlayerCollider;

    protected override void Awake()
    {
        base.Awake();

        GameObject player = GameObject.FindWithTag("Player");
        if (player)
        {
            m_Player = player.transform;
            m_PlayerCollider = player.GetComponent<Collider2D>();
        }
    }

    protected override void OnUpdateAI()
    {
        if (!m_Player || !m_Player.gameObject.activeInHierarchy) return;

        Vector2 playerCenter = m_PlayerCollider
            ? (Vector2)m_PlayerCollider.bounds.center
            : (Vector2)m_Player.position;

        float dist = Vector2.Distance(transform.position, playerCenter);

        if (dist > m_StopDistance)
        {
            m_IsAiming = false;
            MoveTo(m_Player.position);
        }
        else
        {
            StopMoving();
        }

        if (dist <= m_ShootRange)
        {
            m_IsAiming = true;
            RotateToward(playerCenter, m_RotateSpeed);

            if (IsFacing(playerCenter, 15f) && HasClearShot(playerCenter))
                TryShoot();
        }
    }

    private bool IsFacing(Vector2 target, float toleranceDeg)
    {
        Vector2 dir = (target - (Vector2)transform.position).normalized;
        float angle = Vector2.Angle((Vector2)transform.up, dir);
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
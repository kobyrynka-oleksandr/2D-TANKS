using UnityEngine;

public class ScoutAI : EnemyAI
{
    [SerializeField] private LayerMask m_ObstacleMask;

    [Header("Player")]
    [SerializeField] private float m_PlayerStopDistance = 4f;
    [SerializeField] private float m_PlayerShootRange = 6f;

    [Header("Flag")]
    [SerializeField] private float m_FlagStopDistance = 6f;
    [SerializeField] private float m_FlagShootRange = 8f;

    private Transform m_Player;
    private Collider2D m_PlayerCollider;
    private Transform m_FlagTarget;
    private Collider2D m_FlagCollider;

    private bool m_IsChasing;

    protected override void Awake()
    {
        base.Awake();

        GameObject player = GameObject.FindWithTag("Player");
        if (player)
        {
            m_Player = player.transform;
            m_PlayerCollider = player.GetComponent<Collider2D>();
        }

        GameObject flag = GameObject.FindWithTag("Flag");
        if (flag)
        {
            m_FlagTarget = flag.transform;
            m_FlagCollider = flag.GetComponent<Collider2D>();
        }
    }

    protected override void OnUpdateAI()
    {
        if (m_IsChasing && m_Player && m_Player.gameObject.activeInHierarchy)
            HandleChase();
        else
            HandleFlag();
    }

    private void HandleChase()
    {
        Vector2 playerCenter = m_PlayerCollider
            ? (Vector2)m_PlayerCollider.bounds.center
            : (Vector2)m_Player.position;

        float dist = Vector2.Distance(transform.position, playerCenter);

        if (dist > m_PlayerStopDistance)
        {
            m_IsAiming = false;
            MoveTo(m_Player.position);
        }
        else
        {
            StopMoving();
        }

        if (dist <= m_PlayerShootRange)
        {
            m_IsAiming = true;
            RotateToward(playerCenter, m_RotateSpeed);

            if (IsFacing(playerCenter, 15f) && HasClearShot(playerCenter))
                TryShoot();
        }
    }

    private void HandleFlag()
    {
        if (!m_FlagTarget || !m_FlagTarget.gameObject.activeInHierarchy) return;

        Vector2 flagCenter = m_FlagCollider
            ? (Vector2)m_FlagCollider.bounds.center
            : (Vector2)m_FlagTarget.position;

        float distToFlag = Vector2.Distance(transform.position, flagCenter);

        if (distToFlag > m_FlagStopDistance)
        {
            m_IsAiming = false;
            MoveTo(m_FlagTarget.position);
        }
        else
        {
            StopMoving();
        }

        if (distToFlag <= m_FlagShootRange)
        {
            m_IsAiming = true;
            RotateToward(flagCenter, m_RotateSpeed);

            if (IsFacing(flagCenter, 15f) && HasClearShot(flagCenter))
                TryShoot();
        }
    }

    public void OnPlayerEnteredAggro() => m_IsChasing = true;
    public void OnPlayerExitedChase() => m_IsChasing = false;

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
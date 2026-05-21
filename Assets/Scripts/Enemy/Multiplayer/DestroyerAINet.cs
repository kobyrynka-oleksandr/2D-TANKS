using UnityEngine;

public class DestroyerAINet : EnemyAINet
{
    [SerializeField] private float m_ShootAtFlagRange = 8f;
    [SerializeField] private float m_StopDistance = 6f;
    [SerializeField] private LayerMask m_ObstacleMask;

    protected override void OnUpdateAI()
    {
        if (m_FlagTarget == null || !m_FlagTarget.gameObject.activeInHierarchy)
        {
            return;
        }

        Vector2 flagCenter = m_FlagCollider != null
            ? (Vector2)m_FlagCollider.bounds.center
            : (Vector2)m_FlagTarget.position;

        float distToFlag = Vector2.Distance(transform.position, flagCenter);

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
            RotateToward(flagCenter, m_RotateSpeed);

            if (IsFacing(flagCenter, 15f) && HasClearShot(flagCenter))
            {
                TryShoot();
            }
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
        Vector2 origin = transform.position;
        Vector2 direction = (target - origin).normalized;
        float distance = Vector2.Distance(origin, target);
        Vector2 rayStart = origin + direction * 0.6f;
        float rayDist = distance - 0.6f - 0.5f;

        if (rayDist <= 0f)
        {
            return true;
        }

        RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, rayDist, m_ObstacleMask);
        Debug.DrawRay(rayStart, direction * rayDist, hit.collider == null ? Color.green : Color.red);

        return hit.collider == null;
    }
}
using FishNet;
using FishNet.Connection;
using UnityEngine;

public class ScoutAINet : EnemyAINet
{
    [SerializeField] private LayerMask m_ObstacleMask;

    [Header("Player")]
    [SerializeField] private float m_PlayerStopDistance = 4f;
    [SerializeField] private float m_PlayerShootRange = 6f;

    [Header("Flag")]
    [SerializeField] private float m_FlagStopDistance = 6f;
    [SerializeField] private float m_FlagShootRange = 8f;

    private bool m_IsChasing;

    protected override void OnUpdateAI()
    {
        Transform playerTarget = m_IsChasing ? GetNearestPlayer() : null;

        if (m_IsChasing && playerTarget != null && playerTarget.gameObject.activeInHierarchy)
        {
            HandleChase(playerTarget);
        }
        else
        {
            HandleFlag();
        }
    }

    private void HandleChase(Transform playerTarget)
    {
        Collider2D playerCollider = playerTarget.GetComponent<Collider2D>();
        Vector2 playerCenter = playerCollider != null
            ? (Vector2)playerCollider.bounds.center
            : (Vector2)playerTarget.position;

        float dist = Vector2.Distance(transform.position, playerCenter);

        if (dist > m_PlayerStopDistance)
        {
            m_IsAiming = false;
            MoveTo(playerTarget.position);
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
            {
                TryShoot();
            }
        }
    }

    private void HandleFlag()
    {
        if (m_FlagTarget == null || !m_FlagTarget.gameObject.activeInHierarchy)
        {
            return;
        }

        Vector2 flagCenter = m_FlagCollider != null
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
            {
                TryShoot();
            }
        }
    }

    public void OnPlayerEnteredAggro()
    {
        m_IsChasing = true;
    }

    public void OnPlayerExitedChase()
    {
        m_IsChasing = false;
    }

    private Transform GetNearestPlayer()
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

    private bool IsFacing(Vector2 target, float toleranceDeg)
    {
        Vector2 dir = (target - (Vector2)transform.position).normalized;
        float angle = Vector2.Angle((Vector2)transform.up, dir);
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
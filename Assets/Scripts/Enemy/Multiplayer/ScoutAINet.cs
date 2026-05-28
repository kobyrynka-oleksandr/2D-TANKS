using UnityEngine;

public class ScoutAINet : EnemyAINet
{
    [SerializeField] private LayerMask m_ObstacleMask;

    [Header("Player")]
    [SerializeField] private float m_PlayerStopDistance = 4f;
    [SerializeField] private float m_PlayerShootRange = 6f;

    [Header("Defense")]
    [SerializeField] private float m_DefenseStopDistance = 6f;
    [SerializeField] private float m_DefenseShootRange = 8f;

    private bool m_IsChasing;

    protected override void OnUpdateAI()
    {
        Transform target;

        if (m_IsChasing)
        {
            Transform playerTarget = GetNearestPlayer();
            Transform defenseTarget = GetNearestDefenseTarget();
            target = GetCloserTarget(playerTarget, defenseTarget);
        }
        else
        {
            target = GetNearestDefenseTarget();
        }

        if (target == null || !target.gameObject.activeInHierarchy)
        {
            return;
        }

        Vector2 targetCenter = GetTargetCenter(target);
        float distance = Vector2.Distance(transform.position, targetCenter);

        bool isPlayerTarget = target.CompareTag("Player");
        float stopDistance = isPlayerTarget ? m_PlayerStopDistance : m_DefenseStopDistance;
        float shootRange = isPlayerTarget ? m_PlayerShootRange : m_DefenseShootRange;

        if (distance > stopDistance)
        {
            m_IsAiming = false;
            MoveTo(target.position);
        }
        else
        {
            StopMoving();
        }

        if (distance <= shootRange)
        {
            m_IsAiming = true;
            RotateToward(targetCenter, m_RotateSpeed);

            if (IsFacing(targetCenter, 15f) && HasClearShot(targetCenter, m_ObstacleMask))
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
}
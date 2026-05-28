using UnityEngine;

public class DestroyerAINet : EnemyAINet
{
    [SerializeField] private float m_ShootAtTargetRange = 8f;
    [SerializeField] private float m_StopDistance = 6f;
    [SerializeField] private LayerMask m_ObstacleMask;

    protected override void OnUpdateAI()
    {
        Transform target = GetNearestDefenseTarget();

        if (target == null || !target.gameObject.activeInHierarchy)
        {
            return;
        }

        Vector2 targetCenter = GetTargetCenter(target);
        float distance = Vector2.Distance(transform.position, targetCenter);

        if (distance > m_StopDistance)
        {
            m_IsAiming = false;
            MoveTo(target.position);
        }
        else
        {
            StopMoving();
        }

        if (distance <= m_ShootAtTargetRange)
        {
            m_IsAiming = true;
            RotateToward(targetCenter, m_RotateSpeed);

            if (IsFacing(targetCenter, 15f) && HasClearShot(targetCenter, m_ObstacleMask))
            {
                TryShoot();
            }
        }
    }
}
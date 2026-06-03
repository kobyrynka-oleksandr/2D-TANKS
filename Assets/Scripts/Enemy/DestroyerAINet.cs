using UnityEngine;

public class DestroyerAINet : EnemyAINet
{
    [SerializeField] private float _shootAtTargetRange = 8f;
    [SerializeField] private float _stopDistance = 6f;
    [SerializeField] private LayerMask _obstacleMask;

    protected override void OnUpdateAI()
    {
        Transform target = GetNearestDefenseTarget();

        if (target == null || !target.gameObject.activeInHierarchy)
        {
            return;
        }

        Vector2 targetCenter = GetTargetCenter(target);
        float distance = Vector2.Distance(transform.position, targetCenter);

        if (distance > _stopDistance)
        {
            _isAiming = false;
            MoveTo(target.position);
        }
        else
        {
            StopMoving();
        }

        if (distance <= _shootAtTargetRange)
        {
            _isAiming = true;
            RotateToward(targetCenter, _rotateSpeed);

            if (IsFacing(targetCenter, 15f) && HasClearShot(targetCenter, _obstacleMask))
            {
                TryShoot();
            }
        }
    }
}
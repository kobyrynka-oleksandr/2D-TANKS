using UnityEngine;

public class KillerAINet : EnemyAINet
{
    [SerializeField] private float _stopDistance = 4f;
    [SerializeField] private float _shootRange = 6f;
    [SerializeField] private LayerMask _obstacleMask;

    protected override void OnUpdateAI()
    {
        Transform playerTarget = GetNearestPlayer();
        Transform defenseTarget = GetNearestTurret();
        Transform target = GetCloserTarget(playerTarget, defenseTarget);

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

        if (distance <= _shootRange)
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
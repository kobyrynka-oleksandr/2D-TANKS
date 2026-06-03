using UnityEngine;

public class ScoutAINet : EnemyAINet
{
    [SerializeField] private LayerMask _obstacleMask;

    [Header("Player")]
    [SerializeField] private float _playerStopDistance = 4f;
    [SerializeField] private float _playerShootRange = 6f;

    [Header("Defense")]
    [SerializeField] private float _defenseStopDistance = 6f;
    [SerializeField] private float _defenseShootRange = 8f;

    private bool _isChasing;

    protected override void OnUpdateAI()
    {
        Transform target;

        if (_isChasing)
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
        float stopDistance = isPlayerTarget ? _playerStopDistance : _defenseStopDistance;
        float shootRange = isPlayerTarget ? _playerShootRange : _defenseShootRange;

        if (distance > stopDistance)
        {
            _isAiming = false;
            MoveTo(target.position);
        }
        else
        {
            StopMoving();
        }

        if (distance <= shootRange)
        {
            _isAiming = true;
            RotateToward(targetCenter, _rotateSpeed);

            if (IsFacing(targetCenter, 15f) && HasClearShot(targetCenter, _obstacleMask))
            {
                TryShoot();
            }
        }
    }

    public void OnPlayerEnteredAggro()
    {
        _isChasing = true;
    }

    public void OnPlayerExitedChase()
    {
        _isChasing = false;
    }
}
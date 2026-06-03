using FishNet.Object;
using UnityEngine;

public class PlayerDeathReporterNet : NetworkBehaviour
{
    [SerializeField] private TargetHealthNet _health;

    public override void OnStartServer()
    {
        base.OnStartServer();

        InitializeHealth();

        if (_health == null)
        {
            return;
        }

        SubscribeEvents();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        UnsubscribeEvents();
    }

    [Server]
    private void HandleDeath()
    {
        if (GameManagerNet.Instance == null)
        {
            Debug.LogWarning(
                "PlayerDeathReporterNet: GameManagerNet.Instance is null.");

            return;
        }

        GameManagerNet.Instance.OnPlayerDied();
    }

    private void InitializeHealth()
    {
        if (_health != null)
        {
            return;
        }

        _health = GetComponent<TargetHealthNet>();

        if (_health == null)
        {
            Debug.LogError(
                "PlayerDeathReporterNet: TargetHealthNet not found.");
        }
    }

    private void SubscribeEvents()
    {
        _health.DeathEvent += HandleDeath;
    }

    private void UnsubscribeEvents()
    {
        if (_health != null)
        {
            _health.DeathEvent -= HandleDeath;
        }
    }
}
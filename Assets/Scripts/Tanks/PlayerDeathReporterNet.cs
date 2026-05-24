using FishNet.Object;
using UnityEngine;

public class PlayerDeathReporterNet : NetworkBehaviour
{
    [SerializeField] private TargetHealthNet m_Health;

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (m_Health == null)
        {
            m_Health = GetComponent<TargetHealthNet>();
        }

        if (m_Health == null)
        {
            Debug.LogError("PlayerDeathReporterNet: TargetHealthNet not found.");
            return;
        }

        m_Health.OnDeath += HandleDeath;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        if (m_Health != null)
        {
            m_Health.OnDeath -= HandleDeath;
        }
    }

    [Server]
    private void HandleDeath()
    {
        if (GameManagerNet.Instance == null)
        {
            Debug.LogWarning("PlayerDeathReporterNet: GameManagerNet.Instance is null.");
            return;
        }

        GameManagerNet.Instance.OnPlayerDied();
    }
}
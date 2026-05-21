using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.UI;

public class TargetHealthNet : NetworkBehaviour
{
    [SerializeField] private float m_StartingHealth = 100f;
    [SerializeField] private Slider m_Slider;
    [SerializeField] private ParticleSystem m_ExplosionParticles;

    public event System.Action OnDeath;

    private readonly SyncVar<float> m_CurrentHealth = new();
    private bool m_Dead;

    private void Awake()
    {
        if (m_Slider != null)
        {
            m_Slider.maxValue = m_StartingHealth;
        }

        m_CurrentHealth.OnChange += OnHealthChanged;
    }

    private void OnDestroy()
    {
        m_CurrentHealth.OnChange -= OnHealthChanged;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        m_CurrentHealth.Value = m_StartingHealth;
        m_Dead = false;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        SetHealthUI(m_CurrentHealth.Value);
    }

    [Server]
    public void TakeDamage(float amount)
    {
        if (m_Dead)
        {
            return;
        }

        m_CurrentHealth.Value -= amount;

        if (m_CurrentHealth.Value <= 0f)
        {
            Die();
        }
    }

    [Server]
    public void Heal(float amount)
    {
        if (m_Dead)
        {
            return;
        }

        m_CurrentHealth.Value = Mathf.Min(m_CurrentHealth.Value + amount, m_StartingHealth);
    }

    private void OnHealthChanged(float prev, float next, bool asServer)
    {
        SetHealthUI(next);
    }

    private void SetHealthUI(float health)
    {
        if (m_Slider != null)
        {
            m_Slider.value = health;
        }
    }

    [Server]
    private void Die()
    {
        if (m_Dead)
        {
            return;
        }

        m_Dead = true;
        OnDeath?.Invoke();
        RpcPlayDeathEffects();
        Despawn();
    }

    [ObserversRpc]
    private void RpcPlayDeathEffects()
    {
        if (m_ExplosionParticles == null)
        {
            return;
        }

        m_ExplosionParticles.transform.parent = null;
        m_ExplosionParticles.Play();
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
    }
}
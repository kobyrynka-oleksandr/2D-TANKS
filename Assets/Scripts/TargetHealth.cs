using UnityEngine;
using UnityEngine.UI;

public class TargetHealth : MonoBehaviour
{
    [SerializeField] private float m_StartingHealth = 100f;
    [SerializeField] private Slider m_Slider;
    [SerializeField] private ParticleSystem m_ExplosionParticles;

    public event System.Action OnDeath;

    private float m_CurrentHealth;
    private bool m_Dead;

    private void Awake()
    {
        if (m_Slider) m_Slider.maxValue = m_StartingHealth;
    }

    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;
        SetHealthUI();
    }

    public void TakeDamage(float amount)
    {
        if (m_Dead) return;

        m_CurrentHealth -= amount;
        SetHealthUI();

        if (m_CurrentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        m_CurrentHealth = Mathf.Min(m_CurrentHealth + amount, m_StartingHealth);
        SetHealthUI();
    }

    private void SetHealthUI()
    {
        if (m_Slider) m_Slider.value = m_CurrentHealth;
    }

    private void Die()
    {
        m_Dead = true;

        OnDeath?.Invoke();

        m_ExplosionParticles.transform.parent = null;
        m_ExplosionParticles.Play();

        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);

        Destroy(gameObject);
    }
}
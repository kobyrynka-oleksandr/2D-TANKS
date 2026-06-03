using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TargetHealthNet : NetworkBehaviour
{
    [SerializeField] private float _startingHealth = 100f;
    [SerializeField] private Slider _slider;
    [SerializeField] private ParticleSystem _explosionParticles;

    public event Action DeathEvent;

    private readonly SyncVar<float> _currentHealth = new();

    private bool _isDead;

    private Coroutine _healUiCoroutine;

    private void Awake()
    {
        InitializeUi();
        SubscribeEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        _currentHealth.Value = _startingHealth;
        _isDead = false;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        UpdateHealthUi(_currentHealth.Value);
    }

    [Server]
    public void TakeDamage(float amount)
    {
        if (!IsServerInitialized || _isDead)
        {
            return;
        }

        _currentHealth.Value -= amount;

        if (_currentHealth.Value <= 0f)
        {
            Die();
        }
    }

    [Server]
    public void Heal(float amount)
    {
        if (_isDead)
        {
            return;
        }

        _currentHealth.Value = Mathf.Min(_currentHealth.Value + amount, _startingHealth);
    }

    [Server]
    public void ShowHealBonusServer()
    {
        if (Owner == null)
        {
            return;
        }

        TargetShowHealBonus(Owner);
    }

    [Server]
    private void Die()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;

        DeathEvent?.Invoke();

        RpcPlayDeathEffects();

        Despawn();
    }

    [ObserversRpc]
    private void RpcPlayDeathEffects()
    {
        if (_explosionParticles == null)
        {
            return;
        }

        _explosionParticles.transform.parent = null;

        _explosionParticles.Play();

        Destroy(_explosionParticles.gameObject, _explosionParticles.main.duration);
    }

    [TargetRpc]
    private void TargetShowHealBonus(
        NetworkConnection connection)
    {
        if (_healUiCoroutine != null)
        {
            StopCoroutine(_healUiCoroutine);
        }

        _healUiCoroutine = StartCoroutine(ShowHealIconBriefly());
    }

    private IEnumerator ShowHealIconBriefly()
    {
        BonusUIManager.Instance?.ShowHeal();

        yield return new WaitForSeconds(1f);

        BonusUIManager.Instance?.HideHeal();

        _healUiCoroutine = null;
    }

    private void OnHealthChanged(
        float previous,
        float next,
        bool asServer)
    {
        UpdateHealthUi(next);
    }

    private void InitializeUi()
    {
        if (_slider != null)
        {
            _slider.maxValue = _startingHealth;
        }
    }

    private void SubscribeEvents()
    {
        _currentHealth.OnChange += OnHealthChanged;
    }

    private void UnsubscribeEvents()
    {
        _currentHealth.OnChange -= OnHealthChanged;
    }

    private void UpdateHealthUi(float health)
    {
        if (_slider != null)
        {
            _slider.value = health;
        }
    }
}
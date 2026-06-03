using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankShootingNet : NetworkBehaviour
{
    [SerializeField] private Rigidbody2D _shellPrefab;
    [SerializeField] private Transform _fireTransform;
    [SerializeField] private float _baseShellSpeed = 10f;
    [SerializeField] private float _baseShotCooldown = 0.5f;
    [SerializeField] private float _maxDamage = 100f;
    [SerializeField] private float _explosionRadius = 1.5f;
    [SerializeField] private AudioSource _shootingAudio;

    private TankInputUser _inputUser;
    private InputAction _fireAction;

    private float _localCooldownTimer;
    private float _serverNextFireTime;

    private Coroutine _damageCoroutine;

    private readonly SyncVar<float> _shellSpeedMultiplier = new(1f);
    private readonly SyncVar<float> _shotCooldownMultiplier = new(1f);

    public float CurrentShellSpeed =>
        _baseShellSpeed * _shellSpeedMultiplier.Value;

    public float CurrentShotCooldown =>
        _baseShotCooldown * _shotCooldownMultiplier.Value;

    public float DamageMultiplier { get; private set; } = 1f;

    public bool IsComputerControlled { get; set; }

    private void Awake()
    {
        InitializeInputUser();
    }

    private void Update()
    {
        if (!CanProcessLocalInput())
        {
            return;
        }

        UpdateCooldown();
        TryShoot();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!IsOwner)
        {
            return;
        }

        InitializeFireInput();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if (!IsOwner)
        {
            return;
        }

        _fireAction?.Disable();
    }

    [Server]
    public void FireFromAI()
    {
        if (Time.time < _serverNextFireTime)
        {
            return;
        }

        FireInternal(_fireTransform.position, _fireTransform.rotation);
    }

    [Server]
    public void ApplyDoubleDamageBonusServer(float duration)
    {
        if (_damageCoroutine != null)
        {
            StopCoroutine(_damageCoroutine);
        }

        _damageCoroutine = StartCoroutine(DoubleDamageRoutineServer(duration));
    }

    [Server]
    public void ApplyShellSpeedUpgradeServer(
        float multiplier)
    {
        _shellSpeedMultiplier.Value = multiplier;
    }

    [Server]
    public void ApplyShotCooldownUpgradeServer(
        float multiplier)
    {
        _shotCooldownMultiplier.Value = multiplier;
    }

    [ServerRpc]
    private void FireServerRpc(
        Vector2 position,
        Quaternion rotation)
    {
        if (Time.time < _serverNextFireTime)
        {
            return;
        }

        FireInternal(position, rotation);
    }

    [Server]
    private void FireInternal(
        Vector2 position,
        Quaternion rotation)
    {
        _serverNextFireTime = Time.time + CurrentShotCooldown;

        Rigidbody2D shell = Instantiate(_shellPrefab, position, rotation);

        if (!TryInitializeShell(shell, rotation))
        {
            Destroy(shell.gameObject);
            return;
        }

        ServerManager.Spawn(shell.gameObject);

        PlayShootSoundObserversRpc();
    }

    [Server]
    private IEnumerator DoubleDamageRoutineServer(
        float duration)
    {
        DamageMultiplier = 2f;

        TargetShowDoubleDamageBonus(Owner);

        yield return new WaitForSeconds(duration);

        DamageMultiplier = 1f;

        TargetHideDoubleDamageBonus(Owner);

        _damageCoroutine = null;
    }

    [ObserversRpc]
    private void PlayShootSoundObserversRpc()
    {
        _shootingAudio?.Play();
    }

    [TargetRpc]
    private void TargetShowDoubleDamageBonus(
        NetworkConnection connection)
    {
        BonusUIManager.Instance?.ShowDoubleDamage();
    }

    [TargetRpc]
    private void TargetHideDoubleDamageBonus(
        NetworkConnection connection)
    {
        BonusUIManager.Instance?.HideDoubleDamage();
    }

    private void InitializeInputUser()
    {
        _inputUser = GetComponent<TankInputUser>();

        if (_inputUser == null)
        {
            _inputUser = gameObject.AddComponent<TankInputUser>();
        }
    }

    private void InitializeFireInput()
    {
        _fireAction = _inputUser.ActionAsset.FindAction("Fire");

        _fireAction?.Enable();
    }

    private bool CanProcessLocalInput()
    {
        return IsOwner && !IsComputerControlled;
    }

    private void UpdateCooldown()
    {
        if (_localCooldownTimer > 0f)
        {
            _localCooldownTimer -= Time.deltaTime;
        }
    }

    private void TryShoot()
    {
        bool canShoot = _localCooldownTimer <= 0f &&
            (_fireAction?.WasPressedThisFrame() ?? false);

        if (!canShoot)
        {
            return;
        }

        _localCooldownTimer = CurrentShotCooldown;

        FireServerRpc(_fireTransform.position, _fireTransform.rotation);
    }

    private bool TryInitializeShell(
        Rigidbody2D shell,
        Quaternion rotation)
    {
        ShellProjectileNet projectile = shell.GetComponent<ShellProjectileNet>();

        ShellExplosion2DNet explosion = shell.GetComponent<ShellExplosion2DNet>();

        if (projectile == null || explosion == null)
        {
            return false;
        }

        projectile.Initialize((Vector2)(rotation * Vector3.up), CurrentShellSpeed);

        explosion.MaxDamage = _maxDamage * DamageMultiplier;

        explosion.ExplosionRadius = _explosionRadius;

        explosion.Shooter = gameObject;

        return true;
    }
}
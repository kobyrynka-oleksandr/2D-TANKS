using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Object;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankShootingNet : NetworkBehaviour
{
    [SerializeField] private Rigidbody2D m_ShellPrefab;
    [SerializeField] private Transform m_FireTransform;
    [SerializeField] private float m_ShellSpeed = 10f;
    [SerializeField] private float m_ShotCooldown = 0.5f;
    [SerializeField] private float m_MaxDamage = 100f;
    [SerializeField] private float m_ExplosionRadius = 1.5f;
    [SerializeField] private AudioSource m_ShootingAudio;

    public float DamageMultiplier { get; private set; } = 1f;
    public bool m_IsComputerControlled { get; set; }

    private TankInputUser m_InputUser;
    private InputAction m_FireAction;
    private float m_LocalCooldownTimer;
    private float m_ServerNextFireTime;
    private Coroutine m_DamageCoroutine;

    private void Awake()
    {
        m_InputUser = GetComponent<TankInputUser>();

        if (m_InputUser == null)
        {
            m_InputUser = gameObject.AddComponent<TankInputUser>();
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner == false)
        {
            return;
        }

        m_FireAction = m_InputUser.ActionAsset.FindAction("Fire");
        m_FireAction?.Enable();
    }

    private void Update()
    {
        if (IsOwner == false || m_IsComputerControlled == true)
        {
            return;
        }

        if (m_LocalCooldownTimer > 0f)
        {
            m_LocalCooldownTimer -= Time.deltaTime;
        }

        if (m_LocalCooldownTimer <= 0f && (m_FireAction?.WasPressedThisFrame() ?? false))
        {
            m_LocalCooldownTimer = m_ShotCooldown;
            FireServerRpc(m_FireTransform.position, m_FireTransform.rotation);
        }
    }

    [ServerRpc]
    private void FireServerRpc(Vector2 position, Quaternion rotation)
    {
        if (Time.time < m_ServerNextFireTime)
        {
            return;
        }

        FireInternal(position, rotation);
    }

    [Server]
    public void FireFromAI()
    {
        if (Time.time < m_ServerNextFireTime)
        {
            return;
        }

        FireInternal(m_FireTransform.position, m_FireTransform.rotation);
    }

    [Server]
    private void FireInternal(Vector2 position, Quaternion rotation)
    {
        m_ServerNextFireTime = Time.time + m_ShotCooldown;

        Rigidbody2D shell = Instantiate(m_ShellPrefab, position, rotation);

        ShellProjectileNet projectile = shell.GetComponent<ShellProjectileNet>();
        ShellExplosion2DNet explosion = shell.GetComponent<ShellExplosion2DNet>();

        if (projectile == null || explosion == null)
        {
            Destroy(shell.gameObject);
            return;
        }

        projectile.Initialize((Vector2)(rotation * Vector3.up), m_ShellSpeed);
        explosion.m_MaxDamage = m_MaxDamage * DamageMultiplier;
        explosion.m_ExplosionRadius = m_ExplosionRadius;
        explosion.m_Shooter = gameObject;

        ServerManager.Spawn(shell.gameObject);
        PlayShootSoundObserversRpc();
    }

    [ObserversRpc]
    private void PlayShootSoundObserversRpc()
    {
        m_ShootingAudio?.Play();
    }

    [Server]
    public void ApplyDoubleDamageBonusServer(float duration)
    {
        if (m_DamageCoroutine != null)
        {
            StopCoroutine(m_DamageCoroutine);
        }

        m_DamageCoroutine = StartCoroutine(DoubleDamageRoutineServer(duration));
    }

    [Server]
    private IEnumerator DoubleDamageRoutineServer(float duration)
    {
        DamageMultiplier = 2f;
        TargetShowDoubleDamageBonus(Owner);

        yield return new WaitForSeconds(duration);

        DamageMultiplier = 1f;
        TargetHideDoubleDamageBonus(Owner);

        m_DamageCoroutine = null;
    }

    [TargetRpc]
    private void TargetShowDoubleDamageBonus(FishNet.Connection.NetworkConnection conn)
    {
        BonusUIManager.Instance?.ShowDoubleDmg();
    }

    [TargetRpc]
    private void TargetHideDoubleDamageBonus(FishNet.Connection.NetworkConnection conn)
    {
        BonusUIManager.Instance?.HideDoubleDmg();
    }
}
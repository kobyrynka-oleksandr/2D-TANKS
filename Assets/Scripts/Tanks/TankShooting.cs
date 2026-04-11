using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-5)]
public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private Rigidbody2D m_ShellPrefab;
    [SerializeField] private Transform m_FireTransform;
    [SerializeField] private float m_ShellSpeed = 10f;
    [SerializeField] private float m_ShotCooldown = 0.5f;

    [SerializeField] private float m_MaxDamage = 100f;
    [SerializeField] private float m_ExplosionRadius = 1.5f;

    [SerializeField] private AudioSource m_ShootingAudio;

    private TankInputUser m_InputUser;
    private InputAction m_FireAction;
    private float m_CooldownTimer;

    public bool m_IsComputerControlled { get; set; } = false;

    private void Awake()
    {
        m_InputUser = GetComponent<TankInputUser>();
        if (m_InputUser == null)
            m_InputUser = gameObject.AddComponent<TankInputUser>();
    }

    private void Start()
    {
        m_FireAction = m_InputUser.ActionAsset.FindAction("Fire");
        m_FireAction?.Enable();
    }

    private void OnDisable()
    {
        m_CooldownTimer = 0f;
    }

    private void Update()
    {
        if (m_CooldownTimer > 0f)
            m_CooldownTimer -= Time.deltaTime;

        if (m_IsComputerControlled) return;

        if (m_CooldownTimer <= 0f && (m_FireAction?.WasPressedThisFrame() ?? false))
            Fire();
    }

    public void Fire()
    {
        if (m_CooldownTimer > 0f) return;

        Rigidbody2D shell = Instantiate(m_ShellPrefab, m_FireTransform.position, m_FireTransform.rotation);
        shell.linearVelocity = (Vector2)m_FireTransform.up * m_ShellSpeed;

        ShellExplosion2D explosion = shell.GetComponent<ShellExplosion2D>();
        explosion.m_MaxDamage = m_MaxDamage;
        explosion.m_ExplosionRadius = m_ExplosionRadius;
        explosion.m_Shooter = gameObject;

        m_ShootingAudio.Play();

        m_CooldownTimer = m_ShotCooldown;
    }
}
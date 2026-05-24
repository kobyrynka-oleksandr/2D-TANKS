using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-5)]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(NetworkObject))]
public class PlayerMovementNet : NetworkBehaviour
{
    [SerializeField] private float m_Speed = 5f;
    [SerializeField] private float m_TurnSpeed = 180f;

    private Rigidbody2D m_Rigidbody;
    private TankInputUser m_InputUser;

    private InputAction m_MoveAction;
    private InputAction m_TurnAction;

    private float m_MovementInputValue;
    private float m_TurnInputValue;

    private float m_BaseSpeed;
    private Coroutine m_SpeedCoroutine;

    private readonly SyncVar<float> m_SpeedSync = new();

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_InputUser = GetComponent<TankInputUser>();
        m_SpeedSync.OnChange += OnSpeedChanged;
    }

    private void OnDestroy()
    {
        m_SpeedSync.OnChange -= OnSpeedChanged;
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        m_BaseSpeed = m_Speed;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner == false)
        {
            if (m_InputUser != null)
            {
                m_InputUser.enabled = false;
            }

            return;
        }

        if (m_InputUser == null)
        {
            m_InputUser = gameObject.AddComponent<TankInputUser>();
        }

        m_MoveAction = m_InputUser.ActionAsset.FindAction("Vertical");
        m_TurnAction = m_InputUser.ActionAsset.FindAction("Horizontal");

        m_MoveAction?.Enable();
        m_TurnAction?.Enable();
    }

    private void OnEnable()
    {
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }

    private void OnDisable()
    {
        if (m_Rigidbody != null)
        {
            m_Rigidbody.linearVelocity = Vector2.zero;
            m_Rigidbody.angularVelocity = 0f;
        }
    }

    private void Update()
    {
        if (IsOwner == false)
        {
            return;
        }

        m_MovementInputValue = m_MoveAction?.ReadValue<float>() ?? 0f;
        m_TurnInputValue = m_TurnAction?.ReadValue<float>() ?? 0f;
    }

    private void FixedUpdate()
    {
        if (IsOwner == false)
        {
            return;
        }

        Move();
        Turn();
    }

    private void Move()
    {
        Vector2 movement = (Vector2)transform.up * m_MovementInputValue * m_Speed;
        m_Rigidbody.linearVelocity = movement;
    }

    private void Turn()
    {
        float turnAmount = -m_TurnInputValue * m_TurnSpeed * Time.fixedDeltaTime;
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation + turnAmount);
    }

    [Server]
    public void ApplySpeedBonusServer(float multiplier, float duration)
    {
        if (m_SpeedCoroutine != null)
        {
            StopCoroutine(m_SpeedCoroutine);
        }

        m_SpeedCoroutine = StartCoroutine(SpeedBonusRoutineServer(multiplier, duration));
    }

    [Server]
    private IEnumerator SpeedBonusRoutineServer(float multiplier, float duration)
    {
        m_SpeedSync.Value = m_BaseSpeed * multiplier;
        TargetShowSpeedBonus(Owner);

        yield return new WaitForSeconds(duration);

        m_SpeedSync.Value = m_BaseSpeed;
        TargetHideSpeedBonus(Owner);

        m_SpeedCoroutine = null;
    }

    private void OnSpeedChanged(float prev, float next, bool asServer)
    {
        m_Speed = next;
    }

    [TargetRpc]
    private void TargetShowSpeedBonus(NetworkConnection conn)
    {
        BonusUIManager.Instance?.ShowSpeed();
    }

    [TargetRpc]
    private void TargetHideSpeedBonus(NetworkConnection conn)
    {
        BonusUIManager.Instance?.HideSpeed();
    }
}
using System.Collections;
using FishNet.Object;
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

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_InputUser = GetComponent<TankInputUser>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        m_BaseSpeed = m_Speed;

        if (!IsOwner)
        {
            if (m_InputUser != null)
                m_InputUser.enabled = false;

            return;
        }

        if (m_InputUser == null)
            m_InputUser = gameObject.AddComponent<TankInputUser>();

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
        if (!IsOwner)
            return;

        m_MovementInputValue = m_MoveAction?.ReadValue<float>() ?? 0f;
        m_TurnInputValue = m_TurnAction?.ReadValue<float>() ?? 0f;
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
            return;

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

    public void ApplySpeedBonus(float multiplier, float duration)
    {
        if (!IsServerInitialized && !IsOwner)
            return;

        if (m_SpeedCoroutine != null)
            StopCoroutine(m_SpeedCoroutine);

        m_SpeedCoroutine = StartCoroutine(SpeedBonusRoutine(multiplier, duration));
    }

    private IEnumerator SpeedBonusRoutine(float multiplier, float duration)
    {
        m_Speed = m_BaseSpeed * multiplier;

        if (IsOwner)
            BonusUIManager.Instance?.ShowSpeed();

        yield return new WaitForSeconds(duration);

        m_Speed = m_BaseSpeed;

        if (IsOwner)
            BonusUIManager.Instance?.HideSpeed();

        m_SpeedCoroutine = null;
    }
}
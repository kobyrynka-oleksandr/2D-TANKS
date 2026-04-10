using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-5)]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float m_Speed = 5f;
    [SerializeField] private float m_TurnSpeed = 180f;

    private Rigidbody2D m_Rigidbody;
    private TankInputUser m_InputUser;

    private InputAction m_MoveAction;
    private InputAction m_TurnAction;

    private float m_MovementInputValue;
    private float m_TurnInputValue;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();

        m_InputUser = GetComponent<TankInputUser>();
        if (m_InputUser == null)
            m_InputUser = gameObject.AddComponent<TankInputUser>();
    }

    private void Start()
    {
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
        m_Rigidbody.linearVelocity = Vector2.zero;
        m_Rigidbody.angularVelocity = 0f;
    }

    private void Update()
    {
        m_MovementInputValue = m_MoveAction?.ReadValue<float>() ?? 0f;
        m_TurnInputValue = m_TurnAction?.ReadValue<float>() ?? 0f;
    }

    private void FixedUpdate()
    {
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
}
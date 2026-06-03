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
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _turnSpeed = 180f;

    private Rigidbody2D _rigidbody;
    private TankInputUser _inputUser;

    private InputAction _moveAction;
    private InputAction _turnAction;

    private float _movementInputValue;
    private float _turnInputValue;

    private float _baseSpeed;

    private Coroutine _speedCoroutine;

    private readonly SyncVar<float> _speedSync = new();

    private void Awake()
    {
        CacheComponents();
        SubscribeEvents();
    }

    private void OnEnable()
    {
        ResetInputValues();
    }

    private void OnDisable()
    {
        ResetPhysics();
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        ReadInput();
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        Move();
        Turn();
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        _baseSpeed = _speed;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!IsOwner)
        {
            DisableRemotePlayerInput();
            return;
        }

        InitializeInput();
    }

    [Server]
    public void ApplySpeedBonusServer(
        float multiplier,
        float duration)
    {
        if (_speedCoroutine != null)
        {
            StopCoroutine(_speedCoroutine);
        }

        _speedCoroutine =
            StartCoroutine(
                SpeedBonusRoutineServer(
                    multiplier,
                    duration));
    }

    [Server]
    private IEnumerator SpeedBonusRoutineServer(
        float multiplier,
        float duration)
    {
        _speedSync.Value = _baseSpeed * multiplier;

        TargetShowSpeedBonus(Owner);

        yield return new WaitForSeconds(duration);

        _speedSync.Value = _baseSpeed;

        TargetHideSpeedBonus(Owner);

        _speedCoroutine = null;
    }

    private void Move()
    {
        Vector2 movement = (Vector2)transform.up * _movementInputValue * _speed;

        _rigidbody.linearVelocity = movement;
    }

    private void Turn()
    {
        float turnAmount = -_turnInputValue * _turnSpeed * Time.fixedDeltaTime;

        _rigidbody.MoveRotation(_rigidbody.rotation + turnAmount);
    }

    private void ReadInput()
    {
        _movementInputValue = _moveAction?.ReadValue<float>() ?? 0f;

        _turnInputValue = _turnAction?.ReadValue<float>() ?? 0f;
    }

    private void OnSpeedChanged(
        float previous,
        float next,
        bool asServer)
    {
        _speed = next;
    }

    [TargetRpc]
    private void TargetShowSpeedBonus(
        NetworkConnection connection)
    {
        BonusUIManager.Instance?.ShowSpeed();
    }

    [TargetRpc]
    private void TargetHideSpeedBonus(
        NetworkConnection connection)
    {
        BonusUIManager.Instance?.HideSpeed();
    }

    private void CacheComponents()
    {
        _rigidbody =
            GetComponent<Rigidbody2D>();

        _inputUser =
            GetComponent<TankInputUser>();
    }

    private void SubscribeEvents()
    {
        _speedSync.OnChange += OnSpeedChanged;
    }

    private void UnsubscribeEvents()
    {
        _speedSync.OnChange -= OnSpeedChanged;
    }

    private void InitializeInput()
    {
        if (_inputUser == null)
        {
            _inputUser = gameObject.AddComponent<TankInputUser>();
        }

        _moveAction = _inputUser.ActionAsset.FindAction("Vertical");

        _turnAction = _inputUser.ActionAsset.FindAction("Horizontal");

        _moveAction?.Enable();
        _turnAction?.Enable();
    }

    private void DisableRemotePlayerInput()
    {
        if (_inputUser != null)
        {
            _inputUser.enabled = false;
        }
    }

    private void ResetInputValues()
    {
        _movementInputValue = 0f;
        _turnInputValue = 0f;
    }

    private void ResetPhysics()
    {
        if (_rigidbody == null)
        {
            return;
        }

        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
    }
}
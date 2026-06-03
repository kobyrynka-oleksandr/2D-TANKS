using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class TankInputUser : MonoBehaviour
{
    public InputUser InputUser => _inputUser;
    public InputActionAsset ActionAsset => _localActionAsset;

    private InputUser _inputUser;
    private InputActionAsset _localActionAsset;

    private void Awake()
    {
        InitializeInputAsset();
        CreateInputUser();

        InputSystem.onDeviceChange += DeviceChanged;
    }

    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= DeviceChanged;

        RemoveInputUser();
    }

    public void ActivateScheme(string schemeName)
    {
        _inputUser.ActivateControlScheme(schemeName);
    }

    public void SetNewInputUser(InputUser user)
    {
        if (!user.valid)
        {
            return;
        }

        _inputUser = user;

        _inputUser.AssociateActionsWithUser(_localActionAsset);

        ActivateExistingControlScheme();
    }

    private void InitializeInputAsset()
    {
        _localActionAsset = InputActionAsset.FromJson(InputSystem.actions.ToJson());
    }

    private void CreateInputUser()
    {
        InputUser user = InputUser.PerformPairingWithDevice(Keyboard.current);

        if (Gamepad.current != null)
        {
            user = InputUser.PerformPairingWithDevice(Gamepad.current, user: user);
        }

        SetNewInputUser(user);
    }

    private void DeviceChanged(
        InputDevice device,
        InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added && device is Gamepad)
        {
            _inputUser = InputUser.PerformPairingWithDevice(device, user: _inputUser);
        }
    }

    private void ActivateExistingControlScheme()
    {
        if (_inputUser.controlScheme.HasValue)
        {
            _inputUser.ActivateControlScheme(_inputUser.controlScheme.Value);
        }
    }

    private void RemoveInputUser()
    {
        if (_inputUser.valid)
        {
            _inputUser.UnpairDevicesAndRemoveUser();
        }
    }
}
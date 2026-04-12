using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class TankInputUser : MonoBehaviour
{
    public InputUser InputUser => m_InputUser;
    public InputActionAsset ActionAsset => m_LocalActionAsset;

    private InputUser m_InputUser;
    private InputActionAsset m_LocalActionAsset;

    private void Awake()
    {
        m_LocalActionAsset = InputActionAsset.FromJson(InputSystem.actions.ToJson());

        var user = InputUser.PerformPairingWithDevice(Keyboard.current);

        if (Gamepad.current != null)
            user = InputUser.PerformPairingWithDevice(Gamepad.current, user: user);

        SetNewInputUser(user);

        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;

        if (m_InputUser.valid)
            m_InputUser.UnpairDevicesAndRemoveUser();
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added && device is Gamepad)
        {
            m_InputUser = InputUser.PerformPairingWithDevice(device, user: m_InputUser);
        }
    }

    public void ActivateScheme(string name)
    {
        m_InputUser.ActivateControlScheme(name);
    }

    public void SetNewInputUser(InputUser user)
    {
        if (!user.valid) return;

        m_InputUser = user;
        m_InputUser.AssociateActionsWithUser(m_LocalActionAsset);

        if (m_InputUser.controlScheme.HasValue)
            m_InputUser.ActivateControlScheme(m_InputUser.controlScheme.Value);
    }
}
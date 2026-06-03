using FishNet;
using FishNet.Transporting;
using TMPro;
using UnityEngine;

public class LobbyMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _addressInput;
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject _lobbyPanel;
    [SerializeField] private TMP_Text _statusText;
    [SerializeField] private TMP_Text _roomIdText;
    [SerializeField] private RoomManager _roomManager;

    private void OnEnable()
    {
        if (InstanceFinder.ClientManager != null)
        {
            InstanceFinder.ClientManager.OnClientConnectionState += ConnectionStateChanged;
        }
    }

    private void OnDisable()
    {
        if (InstanceFinder.ClientManager != null)
        {
            InstanceFinder.ClientManager.OnClientConnectionState -= ConnectionStateChanged;
        }
    }

    public void OnCreateClicked()
    {
        _statusText.text = "Starting server...";
        _roomIdText.text = string.Empty;

        _roomManager.CreateRoom();
    }

    public void OnJoinClicked()
    {
        string address = _addressInput.text;

        if (string.IsNullOrEmpty(address))
        {
            address = "localhost";
        }

        _statusText.text = "Connecting...";
        _roomIdText.text = string.Empty;

        _roomManager.JoinRoom(address);
    }

    private void ConnectionStateChanged(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            HandleConnected();
        }
        else if (args.ConnectionState == LocalConnectionState.Stopped)
        {
            HandleDisconnected();
        }
    }

    private void HandleConnected()
    {
        _statusText.text = "Connected!";

        _menuPanel.SetActive(false);
        _lobbyPanel.SetActive(true);

        _roomIdText.text =
            InstanceFinder.IsServerStarted
            ? _roomManager.CurrentHostAddress
            : string.Empty;
    }

    private void HandleDisconnected()
    {
        _statusText.text = "Disconnected";

        _menuPanel.SetActive(true);
        _lobbyPanel.SetActive(false);

        _roomIdText.text = string.Empty;
    }
}
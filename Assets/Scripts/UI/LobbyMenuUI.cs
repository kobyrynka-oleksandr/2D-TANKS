using FishNet;
using FishNet.Transporting;
using TMPro;
using UnityEngine;

public class LobbyMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_AddressInput;
    [SerializeField] private GameObject m_MenuPanel;
    [SerializeField] private GameObject m_LobbyPanel;
    [SerializeField] private TMP_Text m_StatusText;
    [SerializeField] private RoomManager m_RoomManager;

    private void OnEnable()
    {
        if (InstanceFinder.ClientManager != null)
        {
            InstanceFinder.ClientManager.OnClientConnectionState += OnConnectionState;
        }
    }

    private void OnDisable()
    {
        if (InstanceFinder.ClientManager != null)
        {
            InstanceFinder.ClientManager.OnClientConnectionState -= OnConnectionState;
        }
    }

    public void OnCreateClicked()
    {
        m_StatusText.text = "Starting server...";
        m_RoomManager.CreateRoom();
    }

    public void OnJoinClicked()
    {
        string address = m_AddressInput.text;
        if (string.IsNullOrEmpty(address))
        {
            address = "localhost";
        }
        m_StatusText.text = "Connecting...";
        m_RoomManager.JoinRoom(address);
    }

    private void OnConnectionState(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            m_StatusText.text = "Connected!";
            m_MenuPanel.SetActive(false);
            m_LobbyPanel.SetActive(true);
        }
        else if (args.ConnectionState == LocalConnectionState.Stopped)
        {
            m_StatusText.text = "Disconnected";
            m_MenuPanel.SetActive(true);
            m_LobbyPanel.SetActive(false);
        }
    }
}

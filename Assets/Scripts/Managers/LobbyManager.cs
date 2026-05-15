using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private TMP_Text m_PlayersText;
    [SerializeField] private Button m_StartButton;
    [SerializeField] private GameObject m_PlayerEntryPrefab;
    [SerializeField] private Transform m_ContentParent;

    public override void OnStartServer()
    {
        base.OnStartServer();
        ServerManager.OnRemoteConnectionState += OnRemoteClientConnectionState;
        InstanceFinder.TransportManager.Transport.SetMaximumClients(2);

        m_StartButton.gameObject.SetActive(true);
        m_StartButton.interactable = false;

        ServerUpdateAndBroadcast();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        ServerManager.OnRemoteConnectionState -= OnRemoteClientConnectionState;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        m_StartButton.gameObject.SetActive(IsServerStarted);

        if (!IsServerStarted)
            CmdRequestPlayerList();
    }

    private void OnRemoteClientConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState == RemoteConnectionState.Stopped)
            StartCoroutine(UpdateNextFrame());
        else
            ServerUpdateAndBroadcast();
    }

    private System.Collections.IEnumerator UpdateNextFrame()
    {
        yield return null;
        ServerUpdateAndBroadcast();
    }

    [Server]
    private void ServerUpdateAndBroadcast()
    {
        int count = ServerManager.Clients.Count;

        string[] names = new string[count];
        int i = 0;
        foreach (var client in ServerManager.Clients)
        {
            names[i] = i == 0 ? $"Player {i + 1} (Host)" : $"Player {i + 1}";
            i++;
        }

        RpcUpdatePlayerList(names);
        RpcSetStartButton(count == 2);
    }

    [ServerRpc(RequireOwnership = false)]
    private void CmdRequestPlayerList()
    {
        ServerUpdateAndBroadcast();
    }

    [ObserversRpc(BufferLast = true)]
    private void RpcUpdatePlayerList(string[] names)
    {
        foreach (Transform child in m_ContentParent)
            Destroy(child.gameObject);

        m_PlayersText.text = $"Players: {names.Length}/2";

        for (int i = 0; i < names.Length; i++)
        {
            GameObject entry = Instantiate(m_PlayerEntryPrefab, m_ContentParent);
            TMP_Text nameText = entry.GetComponentInChildren<TMP_Text>();
            nameText.text = names[i];
        }
    }

    [ObserversRpc(BufferLast = true)]
    private void RpcSetStartButton(bool interactable)
    {
        if (IsServerStarted)
            m_StartButton.interactable = interactable;
    }

    [Server]
    public void OnStartGameButton()
    {
        SceneLoadData sld = new SceneLoadData("CityMultiplayer");
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }

    public void OnLeaveRoomButton()
    {
        if (IsServerStarted)
            InstanceFinder.ServerManager.StopConnection(true);
        else
            InstanceFinder.ClientManager.StopConnection();
    }
}
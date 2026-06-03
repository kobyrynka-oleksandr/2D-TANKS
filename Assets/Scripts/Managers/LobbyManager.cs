using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private TMP_Text _playersText;
    [SerializeField] private Button _startButton;
    [SerializeField] private GameObject _playerEntryPrefab;
    [SerializeField] private Transform _contentParent;

    public override void OnStartServer()
    {
        base.OnStartServer();

        SubscribeServerEvents();
        ConfigureServer();

        UpdateStartButtonState(false);
        UpdateAndBroadcastPlayerList();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        UnsubscribeServerEvents();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        _startButton.gameObject.SetActive(IsServerStarted);

        if (!IsServerStarted)
        {
            RequestPlayerList();
        }
    }

    [Server]
    public void OnStartGameButton()
    {
        LoadGameScene();
    }

    public void OnLeaveRoomButton()
    {
        LeaveRoom();
    }

    private void RemoteClientConnectionStateChanged(
        NetworkConnection connection,
        RemoteConnectionStateArgs arguments)
    {
        if (arguments.ConnectionState == RemoteConnectionState.Stopped)
        {
            StartCoroutine(UpdateNextFrame());
            return;
        }

        UpdateAndBroadcastPlayerList();
    }

    private IEnumerator UpdateNextFrame()
    {
        yield return null;

        UpdateAndBroadcastPlayerList();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestPlayerList()
    {
        UpdateAndBroadcastPlayerList();
    }

    [Server]
    private void UpdateAndBroadcastPlayerList()
    {
        string[] playerNames = GeneratePlayerNames();

        RpcUpdatePlayerList(playerNames);
        RpcSetStartButton(playerNames.Length == 2);
    }

    [ObserversRpc(BufferLast = true)]
    private void RpcUpdatePlayerList(string[] playerNames)
    {
        ClearPlayerEntries();

        _playersText.text = $"Players: {playerNames.Length}/2";

        for (int i = 0; i < playerNames.Length; i++)
        {
            CreatePlayerEntry(playerNames[i]);
        }
    }

    [ObserversRpc(BufferLast = true)]
    private void RpcSetStartButton(bool canStart)
    {
        if (IsServerStarted)
        {
            UpdateStartButtonState(canStart);
        }
    }

    private void SubscribeServerEvents()
    {
        ServerManager.OnRemoteConnectionState += RemoteClientConnectionStateChanged;
    }

    private void UnsubscribeServerEvents()
    {
        ServerManager.OnRemoteConnectionState -= RemoteClientConnectionStateChanged;
    }

    private void ConfigureServer()
    {
        InstanceFinder.TransportManager.Transport.SetMaximumClients(2);

        _startButton.gameObject.SetActive(true);
    }

    private void UpdateStartButtonState(bool isInteractable)
    {
        _startButton.interactable = isInteractable;
    }

    private string[] GeneratePlayerNames()
    {
        int playerCount = ServerManager.Clients.Count;

        string[] names = new string[playerCount];

        int index = 0;

        foreach (var client in ServerManager.Clients)
        {
            names[index] =
                index == 0
                ? $"Player {index + 1} (Host)"
                : $"Player {index + 1}";

            index++;
        }

        return names;
    }

    private void ClearPlayerEntries()
    {
        foreach (Transform child in _contentParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreatePlayerEntry(string playerName)
    {
        GameObject entry =
            Instantiate(_playerEntryPrefab, _contentParent);

        TMP_Text playerText =
            entry.GetComponentInChildren<TMP_Text>();

        playerText.text = playerName;
    }

    private void LoadGameScene()
    {
        SceneLoadData sceneLoadData =
            new SceneLoadData("CityMultiplayer");

        sceneLoadData.ReplaceScenes = ReplaceOption.All;

        InstanceFinder.SceneManager.LoadGlobalScenes(sceneLoadData);
    }

    private void LeaveRoom()
    {
        if (IsServerStarted)
        {
            InstanceFinder.ServerManager.StopConnection(true);
            return;
        }

        InstanceFinder.ClientManager.StopConnection();
    }
}
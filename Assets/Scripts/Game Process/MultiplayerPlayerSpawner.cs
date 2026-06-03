using System.Collections;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class MultiplayerPlayerSpawner : MonoBehaviour
{
    [SerializeField] private NetworkObject _playerPrefab;
    [SerializeField] private Transform[] _spawnPoints;

    private bool _isSpawned;

    private IEnumerator Start()
    {
        yield return null;

        if (!InstanceFinder.IsServerStarted || _isSpawned)
        {
            yield break;
        }

        _isSpawned = true;

        int index = 0;

        foreach (NetworkConnection connection in InstanceFinder.ServerManager.Clients.Values)
        {
            if (index >= _spawnPoints.Length)
            {
                break;
            }

            Transform spawnPoint = _spawnPoints[index];
            NetworkObject player = Instantiate(_playerPrefab, spawnPoint.position, spawnPoint.rotation);
            InstanceFinder.ServerManager.Spawn(player, connection);

            index++;
        }
    }
}
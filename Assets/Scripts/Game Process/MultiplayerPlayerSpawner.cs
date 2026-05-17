using System.Collections;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class MultiplayerPlayerSpawner : MonoBehaviour
{
    [SerializeField] private NetworkObject m_PlayerPrefab;
    [SerializeField] private Transform[] m_SpawnPoints;

    private bool m_Spawned;

    private IEnumerator Start()
    {
        yield return null;

        if (!InstanceFinder.IsServerStarted || m_Spawned)
            yield break;

        m_Spawned = true;

        int index = 0;
        foreach (NetworkConnection conn in InstanceFinder.ServerManager.Clients.Values)
        {
            if (index >= m_SpawnPoints.Length)
                break;

            Transform spawnPoint = m_SpawnPoints[index];
            NetworkObject player = Instantiate(m_PlayerPrefab, spawnPoint.position, spawnPoint.rotation);
            InstanceFinder.ServerManager.Spawn(player, conn);

            index++;
        }
    }
}
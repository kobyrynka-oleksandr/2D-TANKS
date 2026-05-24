using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using UnityEngine;

public class BonusSpawnerNet : MonoBehaviour
{
    [SerializeField] private Transform[] m_SpawnPoints;
    [SerializeField] private NetworkObject[] m_BonusPrefabs;
    [SerializeField] private bool m_IsAlwaysSpawnOnTwoPositions = true;

    private readonly List<NetworkObject> m_ActiveBonuses = new();

    [Server]
    public void SpawnForStage()
    {
        ClearBonuses();

        int count = m_IsAlwaysSpawnOnTwoPositions == true ? m_SpawnPoints.Length : Random.Range(1, 3);
        List<int> indices = new List<int>();

        for (int i = 0; i < m_SpawnPoints.Length; i++)
        {
            indices.Add(i);
        }

        for (int i = indices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        for (int i = 0; i < count; i++)
        {
            NetworkObject prefab = m_BonusPrefabs[Random.Range(0, m_BonusPrefabs.Length)];
            Vector3 pos = m_SpawnPoints[indices[i]].position;

            NetworkObject bonus = Instantiate(prefab, pos, Quaternion.identity);
            InstanceFinder.ServerManager.Spawn(bonus.gameObject);
            m_ActiveBonuses.Add(bonus);
        }
    }

    [Server]
    public void ClearBonuses()
    {
        foreach (NetworkObject bonus in m_ActiveBonuses)
        {
            if (bonus != null && bonus.IsSpawned)
            {
                bonus.Despawn();
            }
        }

        m_ActiveBonuses.Clear();
    }
}
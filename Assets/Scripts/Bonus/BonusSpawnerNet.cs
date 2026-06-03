using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using UnityEngine;

public class BonusSpawnerNet : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private NetworkObject[] _bonusPrefabs;
    [SerializeField] private bool _isAlwaysSpawnOnTwoPositions = true;

    private readonly List<NetworkObject> _activeBonuses = new();

    [Server]
    public void SpawnForStage()
    {
        ClearBonuses();

        int spawnCount = _isAlwaysSpawnOnTwoPositions ? _spawnPoints.Length : Random.Range(1, 3);
        List<int> spawnIndices = new List<int>();

        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            spawnIndices.Add(i);
        }

        for (int i = spawnIndices.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (spawnIndices[i], spawnIndices[randomIndex]) = (spawnIndices[randomIndex], spawnIndices[i]);
        }

        for (int i = 0; i < spawnCount; i++)
        {
            NetworkObject bonusPrefab = _bonusPrefabs[Random.Range(0, _bonusPrefabs.Length)];
            Vector3 spawnPosition = _spawnPoints[spawnIndices[i]].position;

            NetworkObject bonusInstance = Instantiate(bonusPrefab, spawnPosition, Quaternion.identity);
            InstanceFinder.ServerManager.Spawn(bonusInstance.gameObject);
            _activeBonuses.Add(bonusInstance);
        }
    }

    [Server]
    public void ClearBonuses()
    {
        foreach (NetworkObject bonus in _activeBonuses)
        {
            if (bonus != null && bonus.IsSpawned)
            {
                bonus.Despawn();
            }
        }

        _activeBonuses.Clear();
    }
}
using System.Collections.Generic;
using UnityEngine;

public class BonusSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] m_SpawnPoints;
    [SerializeField] private GameObject[] m_BonusPrefabs;
    [SerializeField] private bool m_IsAlwaysSpawnOnTwoPositions = true;

    private readonly List<GameObject> m_ActiveBonuses = new();

    public void SpawnForStage()
    {
        ClearBonuses();

        int count = m_IsAlwaysSpawnOnTwoPositions ? m_SpawnPoints.Length : Random.Range(1, 3);
        List<int> indices = new List<int> { 0, 1 };

        for (int i = indices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = m_BonusPrefabs[Random.Range(0, m_BonusPrefabs.Length)];
            Vector3 pos = m_SpawnPoints[indices[i]].position;
            m_ActiveBonuses.Add(Instantiate(prefab, pos, Quaternion.identity));
        }
    }

    public void ClearBonuses()
    {
        foreach (GameObject b in m_ActiveBonuses)
            if (b != null) Destroy(b);
        m_ActiveBonuses.Clear();
    }
}
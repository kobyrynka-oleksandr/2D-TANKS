using FishNet;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SpawnPointNet : MonoBehaviour
{
    [SerializeField] private float m_DelayBetweenSpawns = 1.5f;

    private int m_OccupiedCount;
    private bool m_IsProcessing;

    private readonly Queue<(GameObject prefab, Action onDeath)> m_Queue = new();

    public void Enqueue(GameObject prefab, Action onDeath)
    {
        m_Queue.Enqueue((prefab, onDeath));
        if (!m_IsProcessing)
        {
            StartCoroutine(Process());
        }
    }

    private IEnumerator Process()
    {
        m_IsProcessing = true;

        while (m_Queue.Count > 0)
        {
            if (m_OccupiedCount > 0)
                yield return new WaitForSeconds(m_DelayBetweenSpawns);

            var (prefab, onDeath) = m_Queue.Dequeue();

            GameObject enemy = Instantiate(prefab, transform.position, transform.rotation);
            InstanceFinder.ServerManager.Spawn(enemy);
            m_OccupiedCount++;

            TargetHealthNet health = enemy.GetComponent<TargetHealthNet>();
            if (health != null)
            {
                health.OnDeath += () => m_OccupiedCount--;
                health.OnDeath += onDeath;
            }
        }

        m_IsProcessing = false;
    }
}
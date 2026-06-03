using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using UnityEngine;

public class SpawnPointNet : MonoBehaviour
{
    [SerializeField] private float _delayBetweenSpawns = 1.5f;

    private int _occupiedCount;
    private bool _isProcessing;

    private readonly Queue<(GameObject prefab, Action onDeath)> _queue = new();

    public void Enqueue(GameObject prefab, Action onDeath)
    {
        _queue.Enqueue((prefab, onDeath));

        if (!_isProcessing)
        {
            StartCoroutine(Process());
        }
    }

    private IEnumerator Process()
    {
        _isProcessing = true;

        while (_queue.Count > 0)
        {
            if (_occupiedCount > 0)
            {
                yield return new WaitForSeconds(_delayBetweenSpawns);
            }

            (GameObject prefab, Action onDeath) = _queue.Dequeue();

            GameObject enemy = Instantiate(prefab, transform.position, transform.rotation);
            InstanceFinder.ServerManager.Spawn(enemy);
            _occupiedCount++;

            TargetHealthNet health = enemy.GetComponent<TargetHealthNet>();

            if (health != null)
            {
                health.DeathEvent += () => _occupiedCount--;
                health.DeathEvent += onDeath;
            }
        }

        _isProcessing = false;
    }
}
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using UnityEngine;

public class GameManagerNet : NetworkBehaviour
{
    public static GameManagerNet Instance { get; private set; }

    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private float _stageCooldown = 4f;

    private readonly SyncVar<int> _currentStageSync = new();
    private readonly SyncVar<bool> _isGameOverSync = new();

    private SpawnPointNet[] _spawnPoints;
    private BonusSpawnerNet _bonusSpawner;
    private WorldObjectsSpawnerNet _worldObjectsSpawner;

    private int _enemiesAlive;
    private int _playersAlive;

    private bool _isInitialized;

    public int CurrentStage => _currentStageSync.Value;
    public bool IsGameOver => _isGameOverSync.Value;

    public WorldObjectsSpawnerNet WorldObjectsSpawner => _worldObjectsSpawner;

    public event Action<int> StageChangedEvent;
    public event Action<int> GameOverEvent;

    private void Awake()
    {
        InitializeSingleton();
        ConfigureApplication();
    }

    private void OnEnable()
    {
        SubscribeSyncEvents();
    }

    private void OnDisable()
    {
        UnsubscribeSyncEvents();
        UnsubscribeFlagEvents();
    }

    public void Initialize(
        SpawnPointNet[] spawnPoints,
        BonusSpawnerNet bonusSpawner,
        WorldObjectsSpawnerNet worldObjectsSpawner,
        int playersAlive = 2)
    {
        _spawnPoints = spawnPoints;
        _bonusSpawner = bonusSpawner;
        _worldObjectsSpawner = worldObjectsSpawner;

        _playersAlive = playersAlive;
        _isInitialized = true;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (!_isInitialized)
        {
            Debug.LogError("GameManagerNet: Initialize() must be called before Spawn.");
            return;
        }

        SubscribeFlagEvents();
        ResetGameState();
        StartNextStage();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        StageChangedEvent?.Invoke(_currentStageSync.Value);

        if (_isGameOverSync.Value)
        {
            GameOverEvent?.Invoke(_currentStageSync.Value);
        }
    }

    [Server]
    public void OnEnemyDied()
    {
        if (_isGameOverSync.Value)
        {
            return;
        }

        _enemiesAlive--;

        if (_enemiesAlive <= 0)
        {
            StartCoroutine(NextStageDelay());
        }
    }

    [Server]
    public void OnPlayerDied()
    {
        if (_isGameOverSync.Value)
        {
            return;
        }

        _playersAlive--;

        if (_playersAlive <= 0)
        {
            LoseGame();
        }
    }

    [Server]
    private void StartNextStage()
    {
        if (_isGameOverSync.Value)
        {
            return;
        }

        IncreaseStage();
        SpawnStage(CurrentStage);
        SpawnBonuses();
    }

    [Server]
    private void SpawnStage(int stage)
    {
        if (!ValidateSpawnData())
        {
            return;
        }

        int spawnPointCount = _spawnPoints.Length;

        for (int i = 0; i < stage; i++)
        {
            GameObject enemyPrefab =
                _enemyPrefabs[UnityEngine.Random.Range(0, _enemyPrefabs.Length)];

            SpawnPointNet spawnPoint =
                _spawnPoints[i % spawnPointCount];

            spawnPoint.Enqueue(enemyPrefab, OnEnemyDied);
        }
    }

    [Server]
    private void LoseGame()
    {
        if (_isGameOverSync.Value)
        {
            return;
        }

        _isGameOverSync.Value = true;
        ClearBonuses();
    }

    [Server]
    private void FlagDestroyed()
    {
        LoseGame();
    }

    private IEnumerator NextStageDelay()
    {
        ClearBonuses();

        yield return new WaitForSeconds(_stageCooldown);

        if (!_isGameOverSync.Value)
        {
            StartNextStage();
        }
    }

    private void StageChanged(int previous, int next, bool asServer)
    {
        if (!asServer)
        {
            StageChangedEvent?.Invoke(next);
        }
    }

    private void GameOverChanged(bool previous, bool next, bool asServer)
    {
        if (!asServer && next)
        {
            GameOverEvent?.Invoke(CurrentStage);
        }
    }

    private void InitializeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void ConfigureApplication()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    private void SubscribeSyncEvents()
    {
        _currentStageSync.OnChange += StageChanged;
        _isGameOverSync.OnChange += GameOverChanged;
    }

    private void UnsubscribeSyncEvents()
    {
        _currentStageSync.OnChange -= StageChanged;
        _isGameOverSync.OnChange -= GameOverChanged;
    }

    private void SubscribeFlagEvents()
    {
        if (_worldObjectsSpawner?.FlagHealth != null)
        {
            _worldObjectsSpawner.FlagHealth.DeathEvent += FlagDestroyed;
            return;
        }

        Debug.LogWarning("GameManagerNet: FlagHealth not found.");
    }

    private void UnsubscribeFlagEvents()
    {
        if (_worldObjectsSpawner?.FlagHealth != null)
        {
            _worldObjectsSpawner.FlagHealth.DeathEvent -= FlagDestroyed;
        }
    }

    private void ResetGameState()
    {
        _isGameOverSync.Value = false;
        _currentStageSync.Value = 0;
    }

    private void IncreaseStage()
    {
        _currentStageSync.Value++;
        _enemiesAlive = _currentStageSync.Value;
    }

    private void SpawnBonuses()
    {
        if (_bonusSpawner != null)
        {
            _bonusSpawner.SpawnForStage();
        }
    }

    private void ClearBonuses()
    {
        if (_bonusSpawner != null)
        {
            _bonusSpawner.ClearBonuses();
        }
    }

    private bool ValidateSpawnData()
    {
        if (_spawnPoints == null || _spawnPoints.Length == 0)
        {
            Debug.LogError("GameManagerNet: Spawn points are missing.");
            return false;
        }

        if (_enemyPrefabs == null || _enemyPrefabs.Length == 0)
        {
            Debug.LogError("GameManagerNet: Enemy prefabs are missing.");
            return false;
        }

        return true;
    }
}
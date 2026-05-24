using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using UnityEngine;

public class GameManagerNet : NetworkBehaviour
{
    public static GameManagerNet Instance { get; private set; }

    [SerializeField] private GameObject[] m_EnemyPrefabs;
    [SerializeField] private float m_StageCooldown = 4f;

    private readonly SyncVar<int> m_CurrentStageSync = new();
    private readonly SyncVar<bool> m_IsGameOverSync = new();

    private SpawnPointNet[] m_SpawnPoints;
    private BonusSpawnerNet m_BonusSpawner;
    private WorldObjectsSpawnerNet m_WorldObjectsSpawner;
    private int m_EnemiesAlive;
    private int m_PlayersAlive;
    private bool m_Initialized;

    public int CurrentStage => m_CurrentStageSync.Value;
    public bool IsGameOver => m_IsGameOverSync.Value;

    public event Action<int> OnStageChangedEvent;
    public event Action<int> OnGameOverEvent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    public void Initialize(
        SpawnPointNet[] spawnPoints,
        BonusSpawnerNet bonusSpawner,
        WorldObjectsSpawnerNet worldObjectsSpawner,
        int playersAlive = 2)
    {
        m_SpawnPoints = spawnPoints;
        m_BonusSpawner = bonusSpawner;
        m_WorldObjectsSpawner = worldObjectsSpawner;
        m_PlayersAlive = playersAlive;
        m_Initialized = true;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (m_Initialized == false)
        {
            Debug.LogError("GameManagerNet: Initialize() must be called before Spawn.");
            return;
        }

        if (m_WorldObjectsSpawner != null && m_WorldObjectsSpawner.FlagHealth != null)
        {
            m_WorldObjectsSpawner.FlagHealth.OnDeath += OnFlagDestroyed;
        }
        else
        {
            Debug.LogWarning("GameManagerNet: FlagHealth not found.");
        }

        m_IsGameOverSync.Value = false;
        m_CurrentStageSync.Value = 0;

        StartNextStage();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        OnStageChangedEvent?.Invoke(m_CurrentStageSync.Value);

        if (m_IsGameOverSync.Value == true)
        {
            OnGameOverEvent?.Invoke(m_CurrentStageSync.Value);
        }
    }

    private void OnEnable()
    {
        m_CurrentStageSync.OnChange += OnStageChanged;
        m_IsGameOverSync.OnChange += OnGameOverChanged;
    }

    private void OnDisable()
    {
        m_CurrentStageSync.OnChange -= OnStageChanged;
        m_IsGameOverSync.OnChange -= OnGameOverChanged;

        if (m_WorldObjectsSpawner != null && m_WorldObjectsSpawner.FlagHealth != null)
        {
            m_WorldObjectsSpawner.FlagHealth.OnDeath -= OnFlagDestroyed;
        }
    }

    [Server]
    public void OnEnemyDied()
    {
        if (m_IsGameOverSync.Value == true)
        {
            return;
        }

        m_EnemiesAlive--;

        if (m_EnemiesAlive <= 0)
        {
            StartCoroutine(NextStageDelay());
        }
    }

    [Server]
    public void OnPlayerDied()
    {
        if (m_IsGameOverSync.Value == true)
        {
            return;
        }

        m_PlayersAlive--;

        if (m_PlayersAlive <= 0)
        {
            OnLoss();
        }
    }

    [Server]
    private void OnFlagDestroyed()
    {
        OnLoss();
    }

    [Server]
    private void StartNextStage()
    {
        if (m_IsGameOverSync.Value == true)
        {
            return;
        }

        int nextStage = m_CurrentStageSync.Value + 1;
        m_CurrentStageSync.Value = nextStage;
        m_EnemiesAlive = nextStage;

        SpawnStage(nextStage);

        if (m_BonusSpawner != null)
        {
            m_BonusSpawner.SpawnForStage();
        }
    }

    [Server]
    private void SpawnStage(int stage)
    {
        if (m_SpawnPoints == null || m_SpawnPoints.Length == 0)
        {
            Debug.LogError("GameManagerNet: Spawn points are missing.");
            return;
        }

        if (m_EnemyPrefabs == null || m_EnemyPrefabs.Length == 0)
        {
            Debug.LogError("GameManagerNet: Enemy prefabs are missing.");
            return;
        }

        int pointCount = m_SpawnPoints.Length;

        for (int i = 0; i < stage; i++)
        {
            GameObject prefab = m_EnemyPrefabs[UnityEngine.Random.Range(0, m_EnemyPrefabs.Length)];
            SpawnPointNet point = m_SpawnPoints[i % pointCount];
            point.Enqueue(prefab, OnEnemyDied);
        }
    }

    private IEnumerator NextStageDelay()
    {
        if (m_BonusSpawner != null)
        {
            m_BonusSpawner.ClearBonuses();
        }

        yield return new WaitForSeconds(m_StageCooldown);

        if (m_IsGameOverSync.Value == false)
        {
            StartNextStage();
        }
    }

    [Server]
    private void OnLoss()
    {
        if (m_IsGameOverSync.Value == true)
        {
            return;
        }

        m_IsGameOverSync.Value = true;

        if (m_BonusSpawner != null)
        {
            m_BonusSpawner.ClearBonuses();
        }
    }

    private void OnStageChanged(int prev, int next, bool asServer)
    {
        if (asServer == false)
        {
            OnStageChangedEvent?.Invoke(next);
        }
    }

    private void OnGameOverChanged(bool prev, bool next, bool asServer)
    {
        if (asServer == false && next == true)
        {
            OnGameOverEvent?.Invoke(CurrentStage);
        }
    }
}
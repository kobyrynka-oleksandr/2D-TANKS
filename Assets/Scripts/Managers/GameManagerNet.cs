using FishNet;
using FishNet.Object;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManagerNet : NetworkBehaviour
{
    public static GameManagerNet Instance { get; private set; }

    [SerializeField] private GameObject[] m_EnemyPrefabs;
    [SerializeField] private SpawnPointNet[] m_SpawnPoints;
    [SerializeField] private float m_StageCooldown = 4f;

    [SerializeField] private BonusSpawner m_BonusSpawner;
    [SerializeField] private WorldObjectsSpawnerNet m_WorldObjectsSpawner;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI m_StageText;
    [SerializeField] private GameObject m_GameOverPanel;
    [SerializeField] private GameObject m_GUI;
    [SerializeField] private TextMeshProUGUI m_FinalScore;

    private int m_CurrentStage;
    private int m_EnemiesAlive;
    private bool m_GameOver;

    public int CurrentStage => m_CurrentStage;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (m_WorldObjectsSpawner != null && m_WorldObjectsSpawner.FlagHealth != null)
        {
            m_WorldObjectsSpawner.FlagHealth.OnDeath += OnLoss;
        }
        else
        {
            Debug.LogWarning("GameManagerNet: FlagHealth not found in WorldObjectsSpawnerNet.");
        }

        StartNextStage();
    }

    [Server]
    public void OnEnemyDied()
    {
        if (m_GameOver)
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
        OnLoss();
    }

    [Server]
    private void StartNextStage()
    {
        m_CurrentStage++;
        m_EnemiesAlive = m_CurrentStage;

        SpawnStage();
        m_BonusSpawner?.SpawnForStage();

        RpcUpdateStageUI(m_CurrentStage);
    }

    [Server]
    private void SpawnStage()
    {
        int pointCount = m_SpawnPoints.Length;

        for (int i = 0; i < m_CurrentStage; i++)
        {
            GameObject prefab = m_EnemyPrefabs[Random.Range(0, m_EnemyPrefabs.Length)];
            SpawnPointNet point = m_SpawnPoints[i % pointCount];
            point.Enqueue(prefab, OnEnemyDied);
        }
    }

    private IEnumerator NextStageDelay()
    {
        m_BonusSpawner?.ClearBonuses();
        yield return new WaitForSeconds(m_StageCooldown);
        StartNextStage();
    }

    [Server]
    private void OnLoss()
    {
        if (m_GameOver)
        {
            return;
        }

        m_GameOver = true;
        m_BonusSpawner?.ClearBonuses();
        RpcShowGameOver(m_CurrentStage);
    }

    [ObserversRpc]
    private void RpcUpdateStageUI(int stage)
    {
        if (m_StageText != null)
        {
            m_StageText.text = $"Stage: {stage}";
        }
    }

    [ObserversRpc]
    private void RpcShowGameOver(int finalStage)
    {
        if (m_GUI != null)
        {
            m_GUI.SetActive(false);
        }

        if (m_FinalScore != null)
        {
            m_FinalScore.text = finalStage.ToString();
        }

        if (m_GameOverPanel != null)
        {
            m_GameOverPanel.SetActive(true);
        }
    }
}
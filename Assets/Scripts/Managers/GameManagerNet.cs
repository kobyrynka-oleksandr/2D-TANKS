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

        TargetHealth flag = GameObject.FindWithTag("Flag")?.GetComponent<TargetHealth>();
        if (flag)
        {
            flag.OnDeath += OnLoss;
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
        if (m_StageText)
        {
            m_StageText.text = $"Stage: {stage}";
        }
    }

    [ObserversRpc]
    private void RpcShowGameOver(int finalStage)
    {
        m_GUI.SetActive(false);
        m_FinalScore.text = finalStage.ToString();
        m_GameOverPanel.SetActive(true);
    }
}
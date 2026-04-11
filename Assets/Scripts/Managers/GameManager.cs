using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject[] m_EnemyPrefabs;
    [SerializeField] private SpawnPoint[] m_SpawnPoints;
    [SerializeField] private float m_StageCooldown = 2f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI m_StageText;
    [SerializeField] private GameObject m_GameOverPanel;
    [SerializeField] private GameObject m_GUI;
    [SerializeField] private TextMeshProUGUI m_FinalScore;

    private int m_CurrentStage;
    private int m_EnemiesAlive;
    private bool m_GameOver;

    private TargetHealth m_Player;
    private TargetHealth m_Flag;

    public int CurrentStage => m_CurrentStage;

    private void Awake()
    {
        if (Instance) { Destroy(gameObject); return; }
        Instance = this;

        Time.timeScale = 1f;

        m_Player = GameObject.FindWithTag("Player")?.GetComponent<TargetHealth>();
        m_Flag = GameObject.FindWithTag("Flag")?.GetComponent<TargetHealth>();
    }

    private void OnEnable()
    {
        if (m_Player) m_Player.OnDeath += OnLoss;
        if (m_Flag) m_Flag.OnDeath += OnLoss;
    }

    private void Start()
    {
        StartNextStage();
    }

    private void OnDisable()
    {
        if (m_Player) m_Player.OnDeath -= OnLoss;
        if (m_Flag) m_Flag.OnDeath -= OnLoss;
    }

    private void StartNextStage()
    {
        m_CurrentStage++;
        m_EnemiesAlive = m_CurrentStage;

        UpdateStageUI();
        SpawnStage();
    }

    private void SpawnStage()
    {
        int pointCount = m_SpawnPoints.Length;

        for (int i = 0; i < m_CurrentStage; i++)
        {
            GameObject prefab = m_EnemyPrefabs[Random.Range(0, m_EnemyPrefabs.Length)];
            SpawnPoint point = m_SpawnPoints[i % pointCount];

            point.Enqueue(prefab, OnEnemyDied);
        }
    }

    private void OnEnemyDied()
    {
        if (m_GameOver) return;

        m_EnemiesAlive--;
        if (m_EnemiesAlive <= 0)
            StartCoroutine(NextStageDelay());
    }

    private IEnumerator NextStageDelay()
    {
        yield return new WaitForSeconds(m_StageCooldown);
        StartNextStage();
    }

    private void UpdateStageUI()
    {
        if (m_StageText) m_StageText.text = $"Stage: {m_CurrentStage}";
    }

    private void OnLoss()
    {
        m_GameOver = true;

        m_GUI.SetActive(false);
        m_FinalScore.text = m_CurrentStage.ToString();
        m_GameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }
}
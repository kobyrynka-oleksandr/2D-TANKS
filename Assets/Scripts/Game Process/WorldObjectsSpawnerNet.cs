using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectsSpawnerNet : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private NetworkObject m_FlagPrefab;
    [SerializeField] private NetworkObject m_BoxPrefab;
    [SerializeField] private NetworkObject m_TurretPrefab;
    [SerializeField] private GameManagerNet m_GameManagerPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform m_FlagSpawnPoint;
    [SerializeField] private Transform[] m_BoxSpawnPoints;
    [SerializeField] private Transform[] m_TurretSpawnPoints;
    [SerializeField] private SpawnPointNet[] m_EnemySpawnPoints;

    [Header("Scene References")]
    [SerializeField] private BonusSpawnerNet m_BonusSpawner;

    [Header("Settings")]
    [SerializeField] private int m_StartingPlayersAlive = 2;

    private readonly List<AutoTurretNet> m_Turrets = new();

    private TargetHealthNet m_FlagHealth;
    private GameManagerNet m_GameManager;
    private bool m_IsSpawned;

    public TargetHealthNet FlagHealth => m_FlagHealth;
    public GameManagerNet GameManager => m_GameManager;
    public IReadOnlyList<AutoTurretNet> Turrets => m_Turrets;

    private void Start()
    {
        StartCoroutine(WaitAndSpawnWorldObjects());
    }

    private IEnumerator WaitAndSpawnWorldObjects()
    {
        while (!InstanceFinder.IsServerStarted)
        {
            yield return null;
        }

        if (m_IsSpawned)
        {
            yield break;
        }

        m_IsSpawned = true;
        SpawnWorldObjects();
    }

    public void SpawnWorldObjects()
    {
        SpawnFlag();
        SpawnBoxes();
        SpawnTurrets();
        SpawnGameManager();
    }

    private void SpawnFlag()
    {
        if (m_FlagPrefab == null || m_FlagSpawnPoint == null)
        {
            Debug.LogWarning("WorldObjectsSpawnerNet: Flag prefab or spawn point is missing.");
            return;
        }

        NetworkObject flagObject = Instantiate(
            m_FlagPrefab,
            m_FlagSpawnPoint.position,
            m_FlagSpawnPoint.rotation);

        InstanceFinder.ServerManager.Spawn(flagObject.gameObject);
        m_FlagHealth = flagObject.GetComponent<TargetHealthNet>();
    }

    private void SpawnBoxes()
    {
        if (m_BoxPrefab == null || m_BoxSpawnPoints == null || m_BoxSpawnPoints.Length == 0)
        {
            return;
        }

        for (int i = 0; i < m_BoxSpawnPoints.Length; i++)
        {
            Transform point = m_BoxSpawnPoints[i];

            if (point == null)
            {
                continue;
            }

            NetworkObject boxObject = Instantiate(
                m_BoxPrefab,
                point.position,
                point.rotation);

            InstanceFinder.ServerManager.Spawn(boxObject.gameObject);
        }
    }

    private void SpawnTurrets()
    {
        m_Turrets.Clear();

        if (m_TurretPrefab == null || m_TurretSpawnPoints == null || m_TurretSpawnPoints.Length == 0)
        {
            return;
        }

        for (int i = 0; i < m_TurretSpawnPoints.Length; i++)
        {
            Transform point = m_TurretSpawnPoints[i];

            if (point == null)
            {
                continue;
            }

            NetworkObject turretObject = Instantiate(
                m_TurretPrefab,
                point.position,
                point.rotation);

            InstanceFinder.ServerManager.Spawn(turretObject.gameObject);

            AutoTurretNet turret = turretObject.GetComponent<AutoTurretNet>();

            if (turret != null)
            {
                m_Turrets.Add(turret);
            }
        }
    }

    private void SpawnGameManager()
    {
        if (m_GameManagerPrefab == null)
        {
            Debug.LogWarning("WorldObjectsSpawnerNet: GameManager prefab is missing.");
            return;
        }

        m_GameManager = Instantiate(m_GameManagerPrefab);
        m_GameManager.Initialize(m_EnemySpawnPoints, m_BonusSpawner, this, m_StartingPlayersAlive);

        InstanceFinder.ServerManager.Spawn(m_GameManager.gameObject);
    }
}
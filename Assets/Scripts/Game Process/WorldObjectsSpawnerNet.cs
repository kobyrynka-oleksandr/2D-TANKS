using FishNet;
using FishNet.Object;
using UnityEngine;

public class WorldObjectsSpawnerNet : NetworkBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private NetworkObject m_FlagPrefab;
    [SerializeField] private NetworkObject m_BoxPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform m_FlagSpawnPoint;
    [SerializeField] private Transform[] m_BoxSpawnPoints;

    private TargetHealthNet m_FlagHealth;

    public TargetHealthNet FlagHealth => m_FlagHealth;

    public override void OnStartServer()
    {
        base.OnStartServer();

        SpawnFlag();
        SpawnBoxes();
    }

    [Server]
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

        if (m_FlagHealth == null)
        {
            Debug.LogWarning("WorldObjectsSpawnerNet: Spawned flag has no TargetHealthNet.");
        }
    }

    [Server]
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
}
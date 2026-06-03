using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using UnityEngine;

public class WorldObjectsSpawnerNet : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private NetworkObject _flagPrefab;
    [SerializeField] private NetworkObject _boxPrefab;
    [SerializeField] private NetworkObject _turretPrefab;
    [SerializeField] private GameManagerNet _gameManagerPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform _flagSpawnPoint;
    [SerializeField] private Transform[] _boxSpawnPoints;
    [SerializeField] private Transform[] _turretSpawnPoints;
    [SerializeField] private SpawnPointNet[] _enemySpawnPoints;

    [Header("Scene References")]
    [SerializeField] private BonusSpawnerNet _bonusSpawner;

    [Header("Settings")]
    [SerializeField] private int _startingPlayersAlive = 2;

    private readonly List<AutoTurretNet> _turrets = new();

    private TargetHealthNet _flagHealth;
    private GameManagerNet _gameManager;
    private bool _isSpawned;

    public TargetHealthNet FlagHealth => _flagHealth;
    public GameManagerNet GameManager => _gameManager;
    public IReadOnlyList<AutoTurretNet> Turrets => _turrets;

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

        if (_isSpawned)
        {
            yield break;
        }

        _isSpawned = true;
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
        if (_flagPrefab == null || _flagSpawnPoint == null)
        {
            Debug.LogWarning("WorldObjectsSpawnerNet: Flag prefab or spawn point is missing.");
            return;
        }

        NetworkObject flagObject = Instantiate(
            _flagPrefab,
            _flagSpawnPoint.position,
            _flagSpawnPoint.rotation);

        InstanceFinder.ServerManager.Spawn(flagObject.gameObject);
        _flagHealth = flagObject.GetComponent<TargetHealthNet>();
    }

    private void SpawnBoxes()
    {
        if (_boxPrefab == null || _boxSpawnPoints == null || _boxSpawnPoints.Length == 0)
        {
            return;
        }

        for (int i = 0; i < _boxSpawnPoints.Length; i++)
        {
            Transform spawnPoint = _boxSpawnPoints[i];

            if (spawnPoint == null)
            {
                continue;
            }

            NetworkObject boxObject = Instantiate(
                _boxPrefab,
                spawnPoint.position,
                spawnPoint.rotation);

            InstanceFinder.ServerManager.Spawn(boxObject.gameObject);
        }
    }

    private void SpawnTurrets()
    {
        _turrets.Clear();

        if (_turretPrefab == null || _turretSpawnPoints == null || _turretSpawnPoints.Length == 0)
        {
            return;
        }

        for (int i = 0; i < _turretSpawnPoints.Length; i++)
        {
            Transform spawnPoint = _turretSpawnPoints[i];

            if (spawnPoint == null)
            {
                continue;
            }

            NetworkObject turretObject = Instantiate(
                _turretPrefab,
                spawnPoint.position,
                spawnPoint.rotation);

            InstanceFinder.ServerManager.Spawn(turretObject.gameObject);

            AutoTurretNet turret = turretObject.GetComponent<AutoTurretNet>();

            if (turret != null)
            {
                _turrets.Add(turret);
            }
        }
    }

    private void SpawnGameManager()
    {
        if (_gameManagerPrefab == null)
        {
            Debug.LogWarning("WorldObjectsSpawnerNet: GameManager prefab is missing.");
            return;
        }

        _gameManager = Instantiate(_gameManagerPrefab);
        _gameManager.Initialize(_enemySpawnPoints, _bonusSpawner, this, _startingPlayersAlive);

        InstanceFinder.ServerManager.Spawn(_gameManager.gameObject);
    }
}
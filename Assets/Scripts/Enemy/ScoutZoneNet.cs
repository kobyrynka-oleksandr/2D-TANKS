using UnityEngine;

public enum ScoutZoneType
{
    Aggro,
    Chase
}

public class ScoutZoneNet : MonoBehaviour
{
    [SerializeField] private ScoutZoneType _zoneType;

    private ScoutAINet _scout;
    private int _playersInside;

    private void Awake()
    {
        _scout = GetComponentInParent<ScoutAINet>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_scout == null || !_scout.IsServerInitialized)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        _playersInside++;

        if (_zoneType == ScoutZoneType.Aggro)
        {
            _scout.OnPlayerEnteredAggro();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_scout == null || !_scout.IsServerInitialized)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        _playersInside = Mathf.Max(0, _playersInside - 1);

        if (_zoneType == ScoutZoneType.Chase && _playersInside == 0)
        {
            _scout.OnPlayerExitedChase();
        }
    }
}
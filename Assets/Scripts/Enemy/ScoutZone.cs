using UnityEngine;

public enum ScoutZoneType { Aggro, Chase }

public class ScoutZone : MonoBehaviour
{
    [SerializeField] private ScoutZoneType m_ZoneType;

    private ScoutAI m_Scout;

    private void Awake()
    {
        m_Scout = GetComponentInParent<ScoutAI>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (m_ZoneType == ScoutZoneType.Aggro) m_Scout.OnPlayerEnteredAggro();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (m_ZoneType == ScoutZoneType.Chase) m_Scout.OnPlayerExitedChase();
    }
}
using UnityEngine;

public enum ScoutZoneType
{
    Aggro,
    Chase
}

public class ScoutZoneNet : MonoBehaviour
{
    [SerializeField] private ScoutZoneType m_ZoneType;

    private ScoutAINet m_Scout;
    private int m_PlayersInside;

    private void Awake()
    {
        m_Scout = GetComponentInParent<ScoutAINet>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (m_Scout == null || !m_Scout.IsServerInitialized)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        m_PlayersInside++;

        if (m_ZoneType == ScoutZoneType.Aggro)
        {
            m_Scout.OnPlayerEnteredAggro();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (m_Scout == null || !m_Scout.IsServerInitialized)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        m_PlayersInside = Mathf.Max(0, m_PlayersInside - 1);

        if (m_ZoneType == ScoutZoneType.Chase && m_PlayersInside == 0)
        {
            m_Scout.OnPlayerExitedChase();
        }
    }
}
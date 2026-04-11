using UnityEngine;

public class CameraPingPong : MonoBehaviour
{
    [SerializeField] private Transform m_PointA;
    [SerializeField] private Transform m_PointB;
    [SerializeField] private float m_Speed = 2f;

    private bool m_GoingToB = true;

    private void Update()
    {
        Transform target = m_GoingToB ? m_PointB : m_PointA;

        transform.position = Vector3.MoveTowards(transform.position, target.position, m_Speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.01f)
            m_GoingToB = !m_GoingToB;
    }
}
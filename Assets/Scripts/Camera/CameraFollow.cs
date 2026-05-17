using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform m_Target;
    [SerializeField] private Vector3 m_Offset = new Vector3(0f, 0f, -10f);

    public void SetTarget(Transform target)
    {
        m_Target = target;
    }

    private void LateUpdate()
    {
        if (!m_Target) return;

        transform.position = m_Target.position + m_Offset;
        transform.rotation = m_Target.rotation;
    }
}
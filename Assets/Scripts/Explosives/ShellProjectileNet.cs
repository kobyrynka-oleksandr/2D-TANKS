using FishNet.Object;
using UnityEngine;

public class ShellProjectileNet : NetworkBehaviour
{
    private Vector2 m_Direction;
    private float m_Speed;
    private bool m_Initialized;

    public void Initialize(Vector2 direction, float speed)
    {
        m_Direction = direction.normalized;
        m_Speed = speed;
        m_Initialized = true;
    }

    private void Update()
    {
        if (!m_Initialized)
        {
            return;
        }

        transform.position += (Vector3)(m_Direction * m_Speed * Time.deltaTime);
    }
}
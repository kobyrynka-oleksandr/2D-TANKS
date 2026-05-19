using FishNet.Object;
using UnityEngine;

public class ShellExplosion2DNet : NetworkBehaviour
{
    public LayerMask m_TargetMask;
    public LayerMask m_CollisionMask;
    public ParticleSystem m_ExplosionParticles;
    public AudioSource m_ExplosionAudio;

    [HideInInspector] public float m_MaxDamage = 100f;
    [HideInInspector] public float m_ExplosionRadius = 5f;
    [HideInInspector] public GameObject m_Shooter;

    private bool m_Exploded = false;

    private void Start()
    {
        if (!IsServerInitialized)
        {
            return;
        }

        Collider2D wallHit = Physics2D.OverlapCircle(transform.position, 0.3f, m_CollisionMask);
        if (wallHit != null)
        {
            Explode();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServerInitialized)
        {
            return;
        }
        if (other.gameObject == m_Shooter)
        {
            return;
        }
        Explode();
    }

    [Server]
    private void Explode()
    {
        if (m_Exploded)
        {
            return;
        }
        m_Exploded = true;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            transform.position, m_ExplosionRadius, m_TargetMask);

        foreach (Collider2D col in colliders)
        {
            if (col.gameObject == m_Shooter)
            {
                continue;
            }

            TargetHealth targetHealth = col.GetComponent<TargetHealth>();
            if (!targetHealth)
            {
                continue;
            }

            Vector2 closestPoint = col.ClosestPoint(transform.position);
            targetHealth.TakeDamage(CalculateDamage(closestPoint));
        }

        RpcPlayExplosion(transform.position);
        ServerManager.Despawn(gameObject);
    }

    [ObserversRpc]
    private void RpcPlayExplosion(Vector2 position)
    {
        m_ExplosionParticles.transform.parent = null;
        m_ExplosionParticles.transform.position = position;
        m_ExplosionParticles.Play();

        m_ExplosionAudio?.Play();

        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
    }

    private float CalculateDamage(Vector2 closestPoint)
    {
        float distance = Vector2.Distance(transform.position, closestPoint);
        float relativeDistance = (m_ExplosionRadius - distance) / m_ExplosionRadius;
        return Mathf.Max(0f, relativeDistance * m_MaxDamage);
    }
}

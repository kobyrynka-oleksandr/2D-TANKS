using FishNet.Object;
using UnityEngine;

public class ShellExplosion2DNet : NetworkBehaviour
{
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private ParticleSystem _explosionParticles;
    [SerializeField] private AudioSource _explosionAudio;

    [HideInInspector] public float MaxDamage = 100f;
    [HideInInspector] public float ExplosionRadius = 5f;
    [HideInInspector] public GameObject Shooter;

    private bool _isExploded;

    private void Start()
    {
        if (!IsServerInitialized)
        {
            return;
        }

        Collider2D wallHit = Physics2D.OverlapCircle(transform.position, 0.3f, _collisionMask);

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

        if (other.GetComponentInParent<NetworkObject>()?.gameObject == Shooter)
        {
            return;
        }

        Explode();
    }

    [Server]
    private void Explode()
    {
        if (_isExploded)
        {
            return;
        }

        _isExploded = true;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            transform.position,
            ExplosionRadius,
            _targetMask);

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject == Shooter)
            {
                continue;
            }

            TargetHealthNet targetHealth = collider.GetComponentInParent<TargetHealthNet>();

            if (targetHealth == null)
            {
                continue;
            }

            if (!targetHealth.IsServerInitialized)
            {
                continue;
            }

            Vector2 closestPoint = collider.ClosestPoint(transform.position);
            float damage = CalculateDamage(closestPoint);

            if (damage <= 0f)
            {
                continue;
            }

            targetHealth.TakeDamage(damage);
        }

        RpcPlayExplosion(transform.position);
        ServerManager.Despawn(gameObject);
    }

    [ObserversRpc]
    private void RpcPlayExplosion(Vector2 position)
    {
        _explosionParticles.transform.parent = null;
        _explosionParticles.transform.position = position;
        _explosionParticles.Play();

        _explosionAudio?.Play();

        Destroy(_explosionParticles.gameObject, _explosionParticles.main.duration);
    }

    private float CalculateDamage(Vector2 closestPoint)
    {
        float distance = Vector2.Distance(transform.position, closestPoint);
        float relativeDistance = (ExplosionRadius - distance) / ExplosionRadius;
        return Mathf.Max(0f, relativeDistance * MaxDamage);
    }
}
using FishNet.Object;
using UnityEngine;

public class ShellProjectileNet : NetworkBehaviour
{
    private Vector2 _direction;
    private float _speed;
    private bool _isInitialized;

    public void Initialize(Vector2 direction, float speed)
    {
        _direction = direction.normalized;
        _speed = speed;
        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized)
        {
            return;
        }

        transform.position += (Vector3)(_direction * _speed * Time.deltaTime);
    }
}
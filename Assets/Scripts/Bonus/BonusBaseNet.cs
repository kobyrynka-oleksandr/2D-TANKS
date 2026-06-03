using FishNet.Object;
using UnityEngine;

public abstract class BonusBaseNet : NetworkBehaviour
{
    [SerializeField] protected string _playerTag = "Player";

    private bool _isPicked;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isPicked)
        {
            return;
        }

        if (!other.CompareTag(_playerTag))
        {
            return;
        }

        NetworkObject playerObject = other.GetComponent<NetworkObject>();

        if (playerObject == null)
        {
            return;
        }

        _isPicked = true;
        Apply(playerObject);
        Despawn();
    }

    [Server]
    protected virtual void Apply(NetworkObject playerObject)
    {
    }
}
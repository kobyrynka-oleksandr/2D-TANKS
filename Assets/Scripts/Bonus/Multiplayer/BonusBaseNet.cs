using FishNet.Object;
using UnityEngine;

public abstract class BonusBaseNet : NetworkBehaviour
{
    [SerializeField] protected string m_PlayerTag = "Player";
    private bool m_IsPicked;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (m_IsPicked == true)
        {
            return;
        }

        if (other.CompareTag(m_PlayerTag) == false)
        {
            return;
        }

        NetworkObject playerObject = other.GetComponent<NetworkObject>();

        if (playerObject == null)
        {
            return;
        }

        m_IsPicked = true;
        Apply(playerObject);
        Despawn();
    }

    [Server]
    protected virtual void Apply(NetworkObject playerObject)
    {
    }
}
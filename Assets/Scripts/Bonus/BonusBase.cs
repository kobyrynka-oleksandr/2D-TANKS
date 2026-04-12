using UnityEngine;

public abstract class BonusBase : MonoBehaviour
{
    [SerializeField] protected string m_PlayerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(m_PlayerTag)) return;
        Apply(other.gameObject);
        Destroy(gameObject);
    }

    protected abstract void Apply(GameObject player);
}
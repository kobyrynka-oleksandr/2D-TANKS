using FishNet.Object;
using UnityEngine;

public class HealBonusNet : BonusBaseNet
{
    [SerializeField] private float m_HealAmount = 200f;

    [Server]
    protected override void Apply(NetworkObject playerObject)
    {
        TargetHealthNet health = playerObject.GetComponent<TargetHealthNet>();

        if (health != null)
        {
            health.Heal(m_HealAmount);
            health.ShowHealBonusServer();
        }
    }
}
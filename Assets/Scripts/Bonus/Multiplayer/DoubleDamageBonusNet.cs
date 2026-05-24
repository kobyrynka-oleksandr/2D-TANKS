using FishNet.Object;
using UnityEngine;

public class DoubleDamageBonusNet : BonusBaseNet
{
    [SerializeField] private float m_Duration = 7f;

    [Server]
    protected override void Apply(NetworkObject playerObject)
    {
        TankShootingNet shooting = playerObject.GetComponent<TankShootingNet>();

        if (shooting != null)
        {
            shooting.ApplyDoubleDamageBonusServer(m_Duration);
        }
    }
}
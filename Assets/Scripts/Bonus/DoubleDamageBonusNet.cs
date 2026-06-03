using FishNet.Object;
using UnityEngine;

public class DoubleDamageBonusNet : BonusBaseNet
{
    [SerializeField] private float _duration = 7f;

    [Server]
    protected override void Apply(NetworkObject playerObject)
    {
        TankShootingNet shooting = playerObject.GetComponent<TankShootingNet>();

        if (shooting != null)
        {
            shooting.ApplyDoubleDamageBonusServer(_duration);
        }
    }
}
using FishNet.Object;
using UnityEngine;

public class SpeedBonusNet : BonusBaseNet
{
    [SerializeField] private float _speedMultiplier = 1.5f;
    [SerializeField] private float _duration = 7f;

    [Server]
    protected override void Apply(NetworkObject playerObject)
    {
        PlayerMovementNet movement = playerObject.GetComponent<PlayerMovementNet>();

        if (movement != null)
        {
            movement.ApplySpeedBonusServer(_speedMultiplier, _duration);
        }
    }
}
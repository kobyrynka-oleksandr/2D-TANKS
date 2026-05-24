using FishNet.Object;
using UnityEngine;

public class SpeedBonusNet : BonusBaseNet
{
    [SerializeField] private float m_SpeedMultiplier = 1.5f;
    [SerializeField] private float m_Duration = 7f;

    [Server]
    protected override void Apply(NetworkObject playerObject)
    {
        PlayerMovementNet movement = playerObject.GetComponent<PlayerMovementNet>();

        if (movement != null)
        {
            movement.ApplySpeedBonusServer(m_SpeedMultiplier, m_Duration);
        }
    }
}
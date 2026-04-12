using UnityEngine;

public class SpeedBonus : BonusBase
{
    [SerializeField] private float m_SpeedMultiplier = 1.5f;
    [SerializeField] private float m_Duration = 7f;

    protected override void Apply(GameObject player)
    {
        if (player.TryGetComponent<PlayerMovement>(out var movement))
            movement.ApplySpeedBonus(m_SpeedMultiplier, m_Duration);
    }
}
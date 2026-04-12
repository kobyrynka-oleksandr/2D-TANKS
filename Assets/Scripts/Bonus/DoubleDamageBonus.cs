// DoubleDamageBonus.cs
using UnityEngine;

public class DoubleDamageBonus : BonusBase
{
    [SerializeField] private float m_Duration = 7f;

    protected override void Apply(GameObject player)
    {
        if (player.TryGetComponent<TankShooting>(out var shooting))
            shooting.ApplyDoubleDamageBonus(m_Duration);
    }
}
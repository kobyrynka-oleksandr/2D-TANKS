using System.Collections;
using UnityEngine;

public class HealBonus : BonusBase
{
    [SerializeField] private float m_HealAmount = 200f;

    protected override void Apply(GameObject player)
    {
        if (player.TryGetComponent<TargetHealth>(out var health))
        {
            health.Heal(m_HealAmount);
            health.StartCoroutine(ShowHealIconBriefly());
        }
    }

    private IEnumerator ShowHealIconBriefly()
    {
        BonusUIManager.Instance?.ShowHeal();
        yield return new WaitForSeconds(1f);
        BonusUIManager.Instance?.HideHeal();
    }
}
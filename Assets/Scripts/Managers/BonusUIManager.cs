using UnityEngine;
using UnityEngine.UI;

public class BonusUIManager : MonoBehaviour
{
    public static BonusUIManager Instance { get; private set; }

    [SerializeField] private GameObject m_HealIcon;
    [SerializeField] private GameObject m_SpeedIcon;
    [SerializeField] private GameObject m_DoubleDamageIcon;

    private void Awake()
    {
        if (Instance) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        HideAll();
    }

    public void ShowHeal() => SetIcon(m_HealIcon, true);
    public void HideHeal() => SetIcon(m_HealIcon, false);
    public void ShowSpeed() => SetIcon(m_SpeedIcon, true);
    public void HideSpeed() => SetIcon(m_SpeedIcon, false);
    public void ShowDoubleDmg() => SetIcon(m_DoubleDamageIcon, true);
    public void HideDoubleDmg() => SetIcon(m_DoubleDamageIcon, false);

    private void SetIcon(GameObject icon, bool active)
    {
        if (icon) icon.SetActive(active);
    }

    public void HideAll()
    {
        HideHeal();
        HideSpeed();
        HideDoubleDmg();
    }
}
using UnityEngine;

public class PlayerUpgradesManager : MonoBehaviour
{
    [SerializeField] private GameObject _upgradesPanel;

    private TankUpgrades _currentTankUpgrades;

    public void SetTankUpgrades(TankUpgrades tankUpgrades)
    {
        _currentTankUpgrades = tankUpgrades;
    }

    public void SelectAutoloader()
    {
        SelectUpgrade(UpgradeType.Autoloader);
    }

    public void SelectReconOptics()
    {
        SelectUpgrade(UpgradeType.ReconOptics);
    }

    public void SelectFastRounds()
    {
        SelectUpgrade(UpgradeType.FastRounds);
    }
    public void ShowPanel()
    {
        if (_upgradesPanel == null)
        {
            Debug.LogWarning("PlayerUpgradesManager: Upgrades panel reference is missing.");
            return;
        }

        _upgradesPanel.SetActive(true);
    }

    public void HidePanel()
    {
        if (_upgradesPanel == null)
        {
            Debug.LogWarning("PlayerUpgradesManager: Upgrades panel reference is missing.");
            return;
        }

        _upgradesPanel.SetActive(false);
    }

    private void SelectUpgrade(UpgradeType upgradeType)
    {
        if (_currentTankUpgrades == null)
        {
            Debug.LogWarning("PlayerUpgradesManager: TankUpgrades reference is missing.");
            return;
        }

        _currentTankUpgrades.ApplyUpgrade(upgradeType);
        HidePanel();
    }
}
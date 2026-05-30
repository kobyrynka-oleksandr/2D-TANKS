using FishNet.Object;
using UnityEngine;

public class PlayerUpgradesBinder : NetworkBehaviour
{
    private TankUpgrades _tankUpgrades;

    private void Awake()
    {
        _tankUpgrades = GetComponent<TankUpgrades>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner == false)
        {
            return;
        }

        StartCoroutine(BindRoutine());
    }

    private System.Collections.IEnumerator BindRoutine()
    {
        PlayerUpgradesManager playerUpgradesManager = null;

        while (playerUpgradesManager == null)
        {
            playerUpgradesManager = FindFirstObjectByType<PlayerUpgradesManager>(FindObjectsInactive.Include);
            yield return null;
        }

        if (_tankUpgrades == null)
        {
            Debug.LogWarning("PlayerUpgradesBinder: TankUpgrades was not found.");
            yield break;
        }

        playerUpgradesManager.SetTankUpgrades(_tankUpgrades);
        playerUpgradesManager.ShowPanel();
    }
}
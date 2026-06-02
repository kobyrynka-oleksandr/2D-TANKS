using FishNet.Object;
using UnityEngine;

public class PlayerUpgradesBinderNet : NetworkBehaviour
{
    private TankUpgradesNet _tankUpgrades;

    private void Awake()
    {
        _tankUpgrades = GetComponent<TankUpgradesNet>();
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
using FishNet.Object;
using Unity.Cinemachine;
using UnityEngine;

public class TankUpgradesNet : NetworkBehaviour
{
    [Header("Upgrade Multipliers")]
    [SerializeField] private float _autoloaderReloadMultiplier = 0.7f;
    [SerializeField] private float _reconOpticsVisionMultiplier = 1.5f;
    [SerializeField] private float _velocityRoundsProjectileSpeedMultiplier = 1.5f;

    [Header("References")]
    [SerializeField] private TankShootingNet _tankShootingNet;
    [SerializeField] private CinemachineCamera _cinemachineCamera;

    public void ApplyUpgrade(UpgradeType upgradeType)
    {
        if (IsOwner == false)
        {
            return;
        }

        ApplyUpgradeServerRpc(upgradeType);
    }

    [ServerRpc]
    private void ApplyUpgradeServerRpc(UpgradeType upgradeType)
    {
        if (upgradeType == UpgradeType.Autoloader)
        {
            _tankShootingNet.ApplyShotCooldownUpgradeServer(_autoloaderReloadMultiplier);
        }
        else if (upgradeType == UpgradeType.FastRounds)
        {
            _tankShootingNet.ApplyShellSpeedUpgradeServer(_velocityRoundsProjectileSpeedMultiplier);
        }
        else if (upgradeType == UpgradeType.ReconOptics)
        {
            TargetApplyReconOptics(Owner);
        }
    }

    [TargetRpc]
    private void TargetApplyReconOptics(FishNet.Connection.NetworkConnection conn)
    {
        ApplyReconOpticsLocal();
    }

    private void ApplyReconOpticsLocal()
    {
        if (_cinemachineCamera == null)
        {
            Debug.LogWarning("TankUpgrades: CinemachineFollow reference is missing.");
            return;
        }

        _cinemachineCamera.Lens.OrthographicSize *= _reconOpticsVisionMultiplier;
    }
}
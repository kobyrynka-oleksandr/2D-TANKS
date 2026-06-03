using System.Collections;
using Unity.Cinemachine;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class PlayerCameraBinder : NetworkBehaviour
{
    [SerializeField] private CinemachineCamera _virtualCamera;
    [SerializeField] private Transform _cameraTarget;

    private bool _isBound;

    public override void OnStartClient()
    {
        base.OnStartClient();
        StartCoroutine(BindRoutine());
    }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        base.OnOwnershipClient(prevOwner);
        StartCoroutine(BindRoutine());
    }

    private IEnumerator BindRoutine()
    {
        for (int i = 0; i < 10 && !_isBound; i++)
        {
            yield return null;
            TryBindCamera();
        }
    }

    private void TryBindCamera()
    {
        if (_isBound || !IsOwner || _virtualCamera == null || _cameraTarget == null)
        {
            return;
        }

        _virtualCamera.Follow = _cameraTarget;
        _virtualCamera.gameObject.SetActive(true);
        _isBound = true;
    }
}
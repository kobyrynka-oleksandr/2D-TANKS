using System.Collections;
using Unity.Cinemachine;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class PlayerCameraBinder : NetworkBehaviour
{
    [SerializeField] private CinemachineCamera m_VirtualCamera;
    [SerializeField] private Transform m_CameraTarget;

    private bool m_IsBound;

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
        for (int i = 0; i < 10 && !m_IsBound; i++)
        {
            yield return null;
            TryBindCamera();
        }
    }

    private void TryBindCamera()
    {
        if (m_IsBound || !IsOwner || m_VirtualCamera == null || m_CameraTarget == null)
            return;

        m_VirtualCamera.Follow = m_CameraTarget;
        m_VirtualCamera.gameObject.SetActive(true);
        m_IsBound = true;
    }
}
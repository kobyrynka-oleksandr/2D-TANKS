using FishNet;
using FishNet.Managing.Scened;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButtonsManager : MonoBehaviour
{
    public void OnMenuButtonNet()
    {
        StartCoroutine(ReturnToMenuRoutine());
    }

    public void OnLobbyButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
    }

    public void StartGameMultiplayer()
    {
        if (InstanceFinder.IsServerStarted == false)
        {
            return;
        }

        SceneLoadData sceneLoadData = new SceneLoadData("CityMultiplayer");
        sceneLoadData.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(sceneLoadData);
    }

    public void StartGameSingleplayer()
    {
        StartCoroutine(StartGameSingleplayerRoutine());
    }

    private IEnumerator StartGameSingleplayerRoutine()
    {
        Time.timeScale = 1f;
        if (InstanceFinder.IsServerStarted || InstanceFinder.IsClientStarted)
        {
            yield return StartCoroutine(StopNetworkRoutine());
        }

        InstanceFinder.TransportManager.Transport.SetMaximumClients(1);

        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection("localhost");

        while (InstanceFinder.IsServerStarted == false || InstanceFinder.IsClientStarted == false)
        {
            yield return null;
        }

        SceneLoadData sceneLoadData = new SceneLoadData("CitySingleplayer");
        sceneLoadData.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(sceneLoadData);
    }

    private IEnumerator ReturnToMenuRoutine()
    {
        Time.timeScale = 1f;

        yield return StartCoroutine(StopNetworkRoutine());

        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu Scene");
    }

    private IEnumerator StopNetworkRoutine()
    {
        if (InstanceFinder.IsHostStarted)
        {
            InstanceFinder.ServerManager.StopConnection(true);
            InstanceFinder.ClientManager.StopConnection();
        }
        else if (InstanceFinder.IsClientStarted)
        {
            InstanceFinder.ClientManager.StopConnection();
        }
        else if (InstanceFinder.IsServerStarted)
        {
            InstanceFinder.ServerManager.StopConnection(true);
        }

        while (InstanceFinder.IsClientStarted || InstanceFinder.IsServerStarted)
        {
            yield return null;
        }
    }
}
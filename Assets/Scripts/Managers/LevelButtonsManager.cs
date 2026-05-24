using FishNet;
using FishNet.Managing.Scened;
using System.Collections;
using UnityEngine;

public class LevelButtonsManager : MonoBehaviour
{
    public void OnCityButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("City");
    }

    public void OnMenuButton()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu Scene");
    }
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
        if (!InstanceFinder.IsServerStarted) return;

        SceneLoadData sld = new SceneLoadData("CityMultiplayer");
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }

    private IEnumerator ReturnToMenuRoutine()
    {
        Time.timeScale = 1f;

        if (InstanceFinder.IsHostStarted == true)
        {
            InstanceFinder.ServerManager.StopConnection(true);
            InstanceFinder.ClientManager.StopConnection();
        }
        else if (InstanceFinder.IsClientStarted == true)
        {
            InstanceFinder.ClientManager.StopConnection();
        }
        else if (InstanceFinder.IsServerStarted == true)
        {
            InstanceFinder.ServerManager.StopConnection(true);
        }

        while (InstanceFinder.IsClientStarted == true || InstanceFinder.IsServerStarted == true)
        {
            yield return null;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu Scene");
    }
}
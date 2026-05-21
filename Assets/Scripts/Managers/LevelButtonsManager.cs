using FishNet;
using FishNet.Managing.Scened;
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
}
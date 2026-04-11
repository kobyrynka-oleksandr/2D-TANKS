using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButtonsManager : MonoBehaviour
{
    public void OnCityButton()
    {
        SceneManager.LoadScene("City");
    }
    public void OnMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu Scene");
    }
}

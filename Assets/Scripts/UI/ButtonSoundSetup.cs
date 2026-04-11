using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonSoundSetup : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RegisterButtons();
    }

    private void RegisterButtons()
    {
        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();
        foreach (var btn in allButtons)
        {
            btn.onClick.RemoveListener(AudioManager.Instance.PlayClick);
            btn.onClick.AddListener(AudioManager.Instance.PlayClick);
        }
    }
}
using UnityEngine;

public class BootstrapLoader : MonoBehaviour
{
    [SerializeField] private string _firstScene = "Menu Scene";

    private void Start()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_firstScene);
    }
}
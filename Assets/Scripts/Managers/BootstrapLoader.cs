using UnityEngine;

public class BootstrapLoader : MonoBehaviour
{
    [SerializeField] private string m_FirstScene = "Menu Scene";

    private void Start()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(m_FirstScene);
    }
}
using System.Collections;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject m_GameOverPanel;
    [SerializeField] private GameObject m_GUI;
    [SerializeField] private TextMeshProUGUI m_FinalScore;

    private GameManagerNet m_Manager;

    private IEnumerator Start()
    {
        if (m_GameOverPanel != null)
        {
            m_GameOverPanel.SetActive(false);
        }

        while (GameManagerNet.Instance == null)
        {
            yield return null;
        }

        m_Manager = GameManagerNet.Instance;
        m_Manager.OnGameOverEvent += ShowGameOver;

        if (m_Manager.IsGameOver == true)
        {
            ShowGameOver(m_Manager.CurrentStage);
        }
    }

    private void OnDestroy()
    {
        if (m_Manager != null)
        {
            m_Manager.OnGameOverEvent -= ShowGameOver;
        }
    }

    private void ShowGameOver(int finalStage)
    {
        Debug.Log($"GameOverUI: Show game over. Final stage = {finalStage}");

        if (m_GUI != null)
        {
            m_GUI.SetActive(false);
        }

        if (m_FinalScore != null)
        {
            m_FinalScore.text = finalStage.ToString();
        }

        if (m_GameOverPanel != null)
        {
            m_GameOverPanel.SetActive(true);
        }
        Time.timeScale = 0f;
    }
}
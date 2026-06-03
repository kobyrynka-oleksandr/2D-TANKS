using System.Collections;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _gui;
    [SerializeField] private TextMeshProUGUI _finalScore;

    private GameManagerNet _manager;

    private IEnumerator Start()
    {
        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(false);
        }

        while (GameManagerNet.Instance == null)
        {
            yield return null;
        }

        _manager = GameManagerNet.Instance;
        _manager.GameOverEvent += ShowGameOver;

        if (_manager.IsGameOver)
        {
            ShowGameOver(_manager.CurrentStage);
        }
    }

    private void OnDestroy()
    {
        if (_manager != null)
        {
            _manager.GameOverEvent -= ShowGameOver;
        }
    }

    private void ShowGameOver(int finalStage)
    {
        Debug.Log($"GameOverUI: Show game over. Final stage = {finalStage}");

        if (_gui != null)
        {
            _gui.SetActive(false);
        }

        if (_finalScore != null)
        {
            _finalScore.text = finalStage.ToString();
        }

        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }
}
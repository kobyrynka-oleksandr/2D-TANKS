using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardPanel : MonoBehaviour
{
    [SerializeField] private GameObject _rowPrefab;
    [SerializeField] private Transform _content;

    private void OnEnable()
    {
        Populate();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Populate()
    {
        foreach (Transform child in _content)
        {
            Destroy(child.gameObject);
        }

        List<PlayerData> leaderboard = SaveSystem.LoadLeaderboard();

        for (int i = 0; i < leaderboard.Count; i++)
        {
            GameObject row = Instantiate(_rowPrefab, _content);
            row.GetComponent<LeaderboardRow>().Set(i + 1, leaderboard[i]);
        }
    }
}
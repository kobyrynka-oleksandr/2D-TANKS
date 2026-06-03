using System.Collections.Generic;
using UnityEngine;

public class UGSLeaderboardPanel : MonoBehaviour
{
    [SerializeField] private GameObject _rowPrefab;
    [SerializeField] private Transform _content;

    private async void OnEnable()
    {
        await Populate();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private async System.Threading.Tasks.Task Populate()
    {
        foreach (Transform child in _content)
        {
            Destroy(child.gameObject);
        }

        List<PlayerData> leaderboard = await UGSLeaderboardService.GetTopScoresAsync(10);

        for (int i = 0; i < leaderboard.Count; i++)
        {
            GameObject row = Instantiate(_rowPrefab, _content);
            row.GetComponent<LeaderboardRow>().Set(i + 1, leaderboard[i]);
        }
    }
}
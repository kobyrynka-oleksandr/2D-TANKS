using System.Collections.Generic;
using UnityEngine;

public class UGSLeaderboardPanel : MonoBehaviour
{
    [SerializeField] private GameObject m_RowPrefab;
    [SerializeField] private Transform m_Content;

    private async void OnEnable()
    {
        await Populate();
    }

    public void Hide() => gameObject.SetActive(false);

    private async System.Threading.Tasks.Task Populate()
    {
        foreach (Transform child in m_Content)
            Destroy(child.gameObject);

        List<PlayerData> leaderboard = await UGSLeaderboardService.GetTopScoresAsync(10);

        for (int i = 0; i < leaderboard.Count; i++)
        {
            GameObject row = Instantiate(m_RowPrefab, m_Content);
            row.GetComponent<LeaderboardRow>().Set(i + 1, leaderboard[i]);
        }
    }
}

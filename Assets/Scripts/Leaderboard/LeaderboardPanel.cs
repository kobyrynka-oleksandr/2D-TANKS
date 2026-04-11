using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LeaderboardPanel : MonoBehaviour
{
    [SerializeField] private GameObject m_RowPrefab;
    [SerializeField] private Transform m_Content;

    private void OnEnable()
    {
        Populate();
    }

    public void Hide() => gameObject.SetActive(false);

    private void Populate()
    {
        foreach (Transform child in m_Content)
            Destroy(child.gameObject);

        List<PlayerData> leaderboard = SaveSystem.LoadLeaderboard();

        for (int i = 0; i < leaderboard.Count; i++)
        {
            GameObject row = Instantiate(m_RowPrefab, m_Content);
            row.GetComponent<LeaderboardRow>().Set(i + 1, leaderboard[i]);
        }
    }
}
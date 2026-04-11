using UnityEngine;
using TMPro;

public class LeaderboardRow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_RankText;
    [SerializeField] private TextMeshProUGUI m_NameText;
    [SerializeField] private TextMeshProUGUI m_StageText;

    public void Set(int rank, PlayerData data)
    {
        m_RankText.text = $"{rank}";
        m_NameText.text = data.Name;
        m_StageText.text = $"{data.Stage}";
    }
}
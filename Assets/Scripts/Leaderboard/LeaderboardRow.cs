using TMPro;
using UnityEngine;

public class LeaderboardRow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _rankText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _stageText;

    public void Set(int rank, PlayerData data)
    {
        _rankText.text = $"{rank}";
        _nameText.text = data.Name;
        _stageText.text = $"{data.Stage}";
    }
}
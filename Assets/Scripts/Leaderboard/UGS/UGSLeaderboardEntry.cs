using TMPro;
using UnityEngine;

public class UGSLeaderboardEntry : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_NameInput;

    public async void Submit()
    {
        string name = m_NameInput.text.Trim();
        if (string.IsNullOrEmpty(name)) return;

        await UGSLeaderboardService.SubmitScoreAsync(
            name, GameManagerNet.Instance.CurrentStage);
    }
}

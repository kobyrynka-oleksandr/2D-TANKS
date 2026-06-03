using TMPro;
using UnityEngine;

public class UGSLeaderboardEntry : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameInput;

    public async void Submit()
    {
        string name = _nameInput.text.Trim();

        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        await UGSLeaderboardService.SubmitScoreAsync(
            name,
            GameManagerNet.Instance.CurrentStage);
    }
}
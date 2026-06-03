using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameInput;

    public void Submit()
    {
        string name = _nameInput.text.Trim();

        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        PlayerData data = new PlayerData(name, GameManagerNet.Instance.CurrentStage);
        SaveSystem.SavePlayer(data);
    }
}
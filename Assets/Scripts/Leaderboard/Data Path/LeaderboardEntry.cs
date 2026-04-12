using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardEntry : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_NameInput;

    public void Submit()
    {
        string name = m_NameInput.text.Trim();
        if (string.IsNullOrEmpty(name)) return;

        PlayerData data = new PlayerData(name, GameManager.Instance.CurrentStage);
        SaveSystem.SavePlayer(data);
    }
}
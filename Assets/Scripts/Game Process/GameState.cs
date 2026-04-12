using UnityEngine;

public class GameState : MonoBehaviour
{
    public void OnPauseStart()
    {
        Time.timeScale = 0f;
        MobileUIControl.Instance?.Hide();
    }
    public void OnPauseEnd()
    {
        Time.timeScale = 1f;
        MobileUIControl.Instance?.Show();
    }
}

using UnityEngine;

public class GameState : MonoBehaviour
{
    public void OnPauseStart()
    {
        Time.timeScale = 0f;
    }
    public void OnPauseEnd()
    {
        Time.timeScale = 1f;
    }
}

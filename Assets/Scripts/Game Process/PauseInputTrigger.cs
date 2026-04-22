using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;   

public class PauseInputTrigger : MonoBehaviour
{
    [SerializeField] private InputActionReference m_PauseAction;
    [SerializeField] private Button m_PauseButton;
    [SerializeField] private Button m_ResumeButton;

    private bool m_IsPaused;
    private float m_LastPauseTime;
    private void OnEnable() => m_PauseAction.action.performed += OnPause;
    private void OnDisable() => m_PauseAction.action.performed -= OnPause;

    private void OnPause(InputAction.CallbackContext ctx)
    {
        if (Time.realtimeSinceStartup - m_LastPauseTime < 0.2f)
            return;

        m_LastPauseTime = Time.realtimeSinceStartup;
        m_IsPaused = !m_IsPaused;

        if (m_IsPaused)
            m_PauseButton.onClick.Invoke();
        else
            m_ResumeButton.onClick.Invoke();
    }
}
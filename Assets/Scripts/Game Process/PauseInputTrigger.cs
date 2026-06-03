using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseInputTrigger : MonoBehaviour
{
    [SerializeField] private InputActionReference _pauseAction;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _resumeButton;

    private bool _isPaused;
    private float _lastPauseTime;

    private void OnEnable()
    {
        _pauseAction.action.performed += OnPause;
    }

    private void OnDisable()
    {
        _pauseAction.action.performed -= OnPause;
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if (Time.realtimeSinceStartup - _lastPauseTime < 0.2f)
        {
            return;
        }

        _lastPauseTime = Time.realtimeSinceStartup;
        _isPaused = !_isPaused;

        if (_isPaused)
        {
            _pauseButton.onClick.Invoke();
        }
        else
        {
            _resumeButton.onClick.Invoke();
        }
    }
}
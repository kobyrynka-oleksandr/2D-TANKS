using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

[DefaultExecutionOrder(-90)]
public class MobileUIControl : MonoBehaviour
{
    public static MobileUIControl Instance { get; private set; }

    [SerializeField] private bool _autoDisableOnNonMobilePlatform = true;

    public InputDevice Device => _control.control.device;

    private OnScreenControl _control;

    void Awake()
    {
        Instance = this;

#if UNITY_ANDROID
        _control = GetComponentInChildren<OnScreenControl>();
#endif

#if !UNITY_ANDROID
        gameObject.SetActive(false);
#endif
    }

    public void Show()
    {
        if (_autoDisableOnNonMobilePlatform && !Application.isMobilePlatform)
        {
            return;
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (_autoDisableOnNonMobilePlatform && !Application.isMobilePlatform)
        {
            return;
        }

        gameObject.SetActive(false);
    }
}
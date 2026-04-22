using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

[DefaultExecutionOrder(-90)]
public class MobileUIControl : MonoBehaviour
{
    public static MobileUIControl Instance { get; private set; }

    public bool AutoDisableOnNonMobilePlatform = true;

    public InputDevice Device => m_Control.control.device;

    private OnScreenControl m_Control;

    void Awake()
    {
        Instance = this;

#if UNITY_ANDROID
        m_Control = GetComponentInChildren<OnScreenControl>();
#endif

#if !UNITY_ANDROID
        gameObject.SetActive(false);
#endif
    }

    public void Show()
    {
        if (AutoDisableOnNonMobilePlatform && !Application.isMobilePlatform)
        {
            return;
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (AutoDisableOnNonMobilePlatform && !Application.isMobilePlatform)
        {
            return;
        }

        gameObject.SetActive(false);
    }
}
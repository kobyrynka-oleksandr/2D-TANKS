using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Slider m_MusicSlider;
    [SerializeField] private Slider m_SfxSlider;
    [SerializeField] private Toggle m_MusicToggle;
    [SerializeField] private Toggle m_SfxToggle;

    private void OnEnable()
    {
        m_MusicSlider.onValueChanged.RemoveAllListeners();
        m_SfxSlider.onValueChanged.RemoveAllListeners();
        m_MusicToggle.onValueChanged.RemoveAllListeners();
        m_SfxToggle.onValueChanged.RemoveAllListeners();

        m_MusicSlider.value = AudioManager.Instance.MusicVolume;
        m_SfxSlider.value = AudioManager.Instance.SfxVolume;
        m_MusicToggle.isOn = !AudioManager.Instance.MusicMuted;
        m_SfxToggle.isOn = !AudioManager.Instance.SfxMuted;

        m_MusicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        m_SfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSfxVolume);
        m_MusicToggle.onValueChanged.AddListener(OnMusicToggle);
        m_SfxToggle.onValueChanged.AddListener(OnSfxToggle);
    }

    private void OnMusicToggle(bool on) => AudioManager.Instance.SetMusicMuted(!on);
    private void OnSfxToggle(bool on) => AudioManager.Instance.SetSfxMuted(!on);
}
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Toggle _musicToggle;
    [SerializeField] private Toggle _sfxToggle;

    private void OnEnable()
    {
        ClearListeners();

        LoadCurrentSettings();

        AddListeners();
    }

    private void ClearListeners()
    {
        _musicSlider.onValueChanged.RemoveAllListeners();
        _sfxSlider.onValueChanged.RemoveAllListeners();

        _musicToggle.onValueChanged.RemoveAllListeners();
        _sfxToggle.onValueChanged.RemoveAllListeners();
    }

    private void LoadCurrentSettings()
    {
        _musicSlider.value = AudioManager.Instance.MusicVolume;
        _sfxSlider.value = AudioManager.Instance.SfxVolume;

        _musicToggle.isOn =
            !AudioManager.Instance.IsMusicMuted;

        _sfxToggle.isOn =
            !AudioManager.Instance.IsSfxMuted;
    }

    private void AddListeners()
    {
        _musicSlider.onValueChanged.AddListener(
            AudioManager.Instance.SetMusicVolume);

        _sfxSlider.onValueChanged.AddListener(
            AudioManager.Instance.SetSfxVolume);

        _musicToggle.onValueChanged.AddListener(
            OnMusicToggle);

        _sfxToggle.onValueChanged.AddListener(
            OnSfxToggle);
    }

    private void OnMusicToggle(bool enabled)
    {
        AudioManager.Instance.SetMusicMuted(!enabled);
    }

    private void OnSfxToggle(bool enabled)
    {
        AudioManager.Instance.SetSfxMuted(!enabled);
    }
}
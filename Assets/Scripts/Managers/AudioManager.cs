using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioClip _buttonClickClip;

    [Header("Base Levels = 100%")]
    [SerializeField] private float _musicBaseDb = -20f;
    [SerializeField] private float _sfxBaseDb = -8f;

    private float _musicVolume = 1f;
    private float _sfxVolume = 1f;

    private bool _isMusicMuted;
    private bool _isSfxMuted;

    public float MusicVolume => _musicVolume;
    public float SfxVolume => _sfxVolume;

    public bool IsMusicMuted => _isMusicMuted;
    public bool IsSfxMuted => _isSfxMuted;

    private void Awake()
    {
        InitializeSingleton();
    }

    public void SetMusicVolume(float value)
    {
        _musicVolume = value;

        if (!_isMusicMuted)
        {
            ApplyMusicVolume();
        }
    }

    public void SetSfxVolume(float value)
    {
        _sfxVolume = value;

        if (!_isSfxMuted)
        {
            ApplySfxVolume();
        }
    }

    public void SetMusicMuted(bool isMuted)
    {
        _isMusicMuted = isMuted;
        ApplyMusicVolume();
    }

    public void SetSfxMuted(bool isMuted)
    {
        _isSfxMuted = isMuted;
        ApplySfxVolume();
    }

    public void PlayMusic(AudioClip clip)
    {
        _musicSource.clip = clip;
        _musicSource.Play();
    }

    public void StopMusic()
    {
        _musicSource.Stop();
    }

    public void PlayClick()
    {
        _sfxSource.PlayOneShot(_buttonClickClip);
    }

    private void InitializeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void ApplyMusicVolume()
    {
        float volume = _isMusicMuted
            ? -80f
            : ConvertToDb(_musicBaseDb, _musicVolume);

        _mixer.SetFloat("MusicVolume", volume);
    }

    private void ApplySfxVolume()
    {
        float volume = _isSfxMuted
            ? -80f
            : ConvertToDb(_sfxBaseDb, _sfxVolume);

        _mixer.SetFloat("SFXVolume", volume);
    }

    private float ConvertToDb(float baseDb, float sliderValue)
    {
        return baseDb + 20f * Mathf.Log10(Mathf.Max(sliderValue, 0.0001f));
    }
}
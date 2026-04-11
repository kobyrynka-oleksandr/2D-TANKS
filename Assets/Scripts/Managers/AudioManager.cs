using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer m_Mixer;
    [SerializeField] private AudioSource m_MusicSource;
    [SerializeField] private AudioSource m_SfxSource;
    [SerializeField] private AudioClip m_ButtonClickClip;

    [Header("Base Levels = 100%")]
    [SerializeField] private float m_MusicBaseDb = -20f;
    [SerializeField] private float m_SfxBaseDb = -8f;

    private float m_MusicVolume = 1f;
    private float m_SfxVolume = 1f;
    private bool m_MusicMuted;
    private bool m_SfxMuted;

    public float MusicVolume => m_MusicVolume;
    public float SfxVolume => m_SfxVolume;
    public bool MusicMuted => m_MusicMuted;
    public bool SfxMuted => m_SfxMuted;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetMusicVolume(float value)
    {
        m_MusicVolume = value;
        if (!m_MusicMuted)
            m_Mixer.SetFloat("MusicVolume", ToDb(m_MusicBaseDb, value));
    }

    public void SetSfxVolume(float value)
    {
        m_SfxVolume = value;
        if (!m_SfxMuted)
            m_Mixer.SetFloat("SFXVolume", ToDb(m_SfxBaseDb, value));
    }

    public void SetMusicMuted(bool muted)
    {
        m_MusicMuted = muted;
        m_Mixer.SetFloat("MusicVolume", muted ? -80f : ToDb(m_MusicBaseDb, m_MusicVolume));
    }

    public void SetSfxMuted(bool muted)
    {
        m_SfxMuted = muted;
        m_Mixer.SetFloat("SFXVolume", muted ? -80f : ToDb(m_SfxBaseDb, m_SfxVolume));
    }

    private float ToDb(float baseDb, float slider)
    {
        return baseDb + 20f * Mathf.Log10(Mathf.Max(slider, 0.0001f));
    }

    public void PlayMusic(AudioClip clip) { m_MusicSource.clip = clip; m_MusicSource.Play(); }
    public void StopMusic() => m_MusicSource.Stop();
    public void PlayClick() => m_SfxSource.PlayOneShot(m_ButtonClickClip);
}
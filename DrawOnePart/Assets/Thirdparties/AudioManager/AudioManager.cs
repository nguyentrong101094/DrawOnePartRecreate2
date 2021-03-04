using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static float DefaultBgmVolume = 1f;
    public static float DefaultSfxVolume = 1;
    public static float DefaultVoiceVolume = 1;
    public static float DefaultBgmVolumeScale = 1f;
    public static float DefaultSfxVolumeScale = 1;
    public static float DefaultVoiceVolumeScale = 1;

    static float bgmVolume = -1;
    static float sfxVolume = -1;
    static float voiceVolume = -1;

    static float bgmVolumeScale = 1;

    public static string PREF_SFX_VOLUME = "SFX_VOLUME";
    public static string PREF_BGM_VOLUME = "BGM_VOLUME";

    public static float BgmVolume
    {
        get
        {
            if (bgmVolume == -1)
            {
                BgmVolume = PlayerPrefs.GetFloat("BGM_VOLUME", DefaultBgmVolume);
            }

            return bgmVolume;
        }

        set
        {
            bgmVolume = value;
            Instance.m_BgmSource.volume = bgmVolume * DefaultBgmVolumeScale * bgmVolumeScale;
            Instance.m_AudioMixer.SetFloat("BGM", PercentToMixerVolume(bgmVolume));
        }
    }

    public static float SfxVolume
    {
        get
        {
            if (sfxVolume == -1)
            {
                SfxVolume = PlayerPrefs.GetFloat("SFX_VOLUME", DefaultSfxVolume);
                //AudioListener.volume = sfxVolume;
            }

            return sfxVolume;
        }

        set
        {
            sfxVolume = value;
            Instance.m_AudioMixer.SetFloat("SFX", PercentToMixerVolume(sfxVolume));
            //AudioListener.volume = sfxVolume;
        }
    }

    public static float VoiceVolume
    {
        get
        {
            if (voiceVolume == -1)
            {
                voiceVolume = PlayerPrefs.GetFloat("VOICE_VOLUME", DefaultVoiceVolume);
            }

            return voiceVolume;
        }

        set
        {
            voiceVolume = value;
        }
    }

    public static void Save()
    {
        PlayerPrefs.SetFloat("BGM_VOLUME", BgmVolume);
        PlayerPrefs.SetFloat("SFX_VOLUME", SfxVolume);
        PlayerPrefs.SetFloat("VOICE_VOLUME", VoiceVolume);
        PlayerPrefs.Save();
    }

    [SerializeField] AudioMixer m_AudioMixer;
    [SerializeField] string m_BgmPath = "Sounds/Bgm";
    [SerializeField] string m_SfxPath = "Sounds/Sfx";
    [SerializeField] string m_VoicePath = "Sounds/Voice";
    [SerializeField] string m_ButtonTapSfx = "sfx_button_tap";
    [SerializeField] AudioSource m_BgmSource;
    [SerializeField] AudioSource m_SfxSource;
    [SerializeField] AudioSource m_VoiceSource;

    protected static AudioManager instance { get; set; }
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gO = Resources.Load<GameObject>("AudioManager");
                instance = Instantiate(gO).GetComponent<AudioManager>();
            }
            return instance;
        }
    }

    Dictionary<string, AudioClip> m_AudioDict = new Dictionary<string, AudioClip>();
    bool m_PauseBgm;

    public string sfxPath
    {
        get { return m_SfxPath; }
    }

    public string bgmPath
    {
        get { return m_BgmPath; }
    }

    public string voicePath
    {
        get { return m_VoicePath; }
    }

    public AudioSource voiceSource
    {
        get { return m_VoiceSource; }
    }

    public void PlayButtonTapSfx()
    {
        PlaySfx(m_ButtonTapSfx);
    }

    /// <summary>
    /// Play a random sfx from the array
    /// </summary>
    public void PlaySfx(string[] array, float volumeScale = 1f)
    {
        if (array.Length > 0)
            PlaySfx(array[UnityEngine.Random.Range(0, array.Length)], volumeScale, true);
    }

    public void PlaySfx(AudioClip[] array, float volumeScale = 1f)
    {
        if (array.Length > 0)
            PlaySfx(array[UnityEngine.Random.Range(0, array.Length)], volumeScale);
    }

    public void PlaySfx(AudioClip audioClip, float volumeScale = 1f)
    {
        m_SfxSource.PlayOneShot(audioClip, DefaultSfxVolumeScale * volumeScale);
    }

    public void PlaySfx(string audioName, float volumeScale = 1f, bool oneShot = true, bool isLoop = false)
    {
        CacheClip(m_SfxPath, audioName);

        m_SfxSource.volume = DefaultSfxVolumeScale * volumeScale;
        if (oneShot)
        {
            m_SfxSource.PlayOneShot(m_AudioDict[audioName], m_SfxSource.volume * volumeScale);
        }
        else
        {
            m_SfxSource.loop = isLoop;
            m_SfxSource.clip = m_AudioDict[audioName];
            m_SfxSource.Play();
        }
    }

    public void StopSfx()
    {
        m_SfxSource.Stop();
    }

    public float GetLength(string audioName)
    {
        return (m_AudioDict.ContainsKey(audioName) ? m_AudioDict[audioName].length : 0);
    }

    public float PlayVoice(AudioClip audioClip, float volumeScale = 1f)
    {
        m_VoiceSource.volume = VoiceVolume * DefaultVoiceVolume * volumeScale;
        m_VoiceSource.PlayOneShot(audioClip, m_VoiceSource.volume);
        return audioClip.length;
    }

    public void PlayVoice(string audioName, float volumeScale = 1f, bool playOneShot = false)
    {
        CacheClip(m_VoicePath, audioName);

        m_VoiceSource.volume = VoiceVolume * DefaultVoiceVolume * volumeScale;
        if (playOneShot)
        {
            m_VoiceSource.PlayOneShot(m_AudioDict[audioName], m_VoiceSource.volume);
        }
        else
        {
            m_VoiceSource.clip = m_AudioDict[audioName];
            m_VoiceSource.Play();
        }
    }

    public void SetVoicePitch(float pitch)
    {
        m_VoiceSource.pitch = pitch;
    }

    public void PlayBgm(AudioClip clip, float volumeScale = 1f)
    {
        bgmVolumeScale = volumeScale;
        m_BgmSource.volume = BgmVolume * DefaultBgmVolumeScale * bgmVolumeScale;
        string audioName = clip.name;
        bool currentClipNull = (m_BgmSource.clip == null);
        bool sameCurrentClip = (!currentClipNull && m_BgmSource.clip.name == audioName);

        if (sameCurrentClip)
        {
            if (!m_BgmSource.isPlaying)
            {
                m_BgmSource.Play();
            }
        }
        else
        {
            if (!currentClipNull)
            {
                StopBgm(true);
            }
            m_BgmSource.clip = clip;
            m_BgmSource.Play();
        }
    }

    public void PlayBgm(string audioName, float volumeScale = 1f)
    {
        //bgmVolumeScale = volumeScale;
        //m_BgmSource.volume = BgmVolume * DefaultBgmVolumeScale * bgmVolumeScale;

        bool currentClipNull = (m_BgmSource.clip == null);
        bool sameCurrentClip = (!currentClipNull && m_BgmSource.clip.name == audioName);

        if (sameCurrentClip)
        {
            if (!m_BgmSource.isPlaying)
            {
                m_BgmSource.Play();
            }
        }
        else
        {
            if (!currentClipNull)
            {
                StopBgm(true);
            }
            var clip = Resources.Load<AudioClip>(System.IO.Path.Combine(m_BgmPath, audioName));
            PlayBgm(clip);
        }
    }

    public void StopBgm(bool clearClip = false)
    {
        if (m_BgmSource.isPlaying)
        {
            m_BgmSource.Stop();
        }

        if (clearClip && m_BgmSource.clip != null)
        {
            Resources.UnloadAsset(m_BgmSource.clip);
            m_BgmSource.clip = null;
        }
    }

    public bool pauseBgm
    {
        set
        {
            m_PauseBgm = value;

            if (m_PauseBgm)
            {
                m_BgmSource.Pause();
            }
            else
            {
                m_BgmSource.UnPause();
            }
        }
    }

    public void CacheClip(AudioClip clip, string audioName)
    {
        if (!m_AudioDict.ContainsKey(audioName))
        {
            m_AudioDict.Add(audioName, clip);
        }
    }

    public void CacheClip(string path, string audioName)
    {
        /*
        if (m_AudioDict.Count > 10) //clear cache if too large
        {
            foreach (var item in m_AudioDict)
            {
                Resources.UnloadAsset(item.Value);
            }
            m_AudioDict.Clear();
        }
        */
        if (!m_AudioDict.ContainsKey(audioName))
        {
            AudioClip clip = Resources.Load<AudioClip>(System.IO.Path.Combine(path, audioName));
            if (clip == null) { Debug.LogError("audioclip not found " + System.IO.Path.Combine(path, audioName)); }
            m_AudioDict.Add(audioName, clip);
        }
    }

    public void CacheSfx(string[] array)
    {
        foreach (var item in array)
        {
            CacheSfx(item);
        }
    }

    public void CacheSfx(string audioName)
    {
        CacheClip(m_SfxPath, audioName);
    }

    public void ClearCacheClip(string audioName, bool unloadClip)
    {
        if (m_AudioDict.ContainsKey(audioName))
        {
            if (unloadClip)
            {
                Resources.UnloadAsset(m_AudioDict[audioName]);
            }
            m_AudioDict.Remove(audioName);
        }
    }

    public void ClearCacheClip()
    {
        foreach (var item in m_AudioDict)
        {
            Resources.UnloadAsset(item.Value);
        }
        m_AudioDict.Clear();
    }

    public void Fadeout()
    {

    }

    static float PercentToMixerVolume(float soundLevel)
    {
        float val = Mathf.Log(Mathf.Max(0.001f, soundLevel)) * 20;
        return val;
    }

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);

            instance = this;
        }
    }

    private void Start()
    {
        InitVolume();
    }

    void InitVolume()
    {
        if (bgmVolume == -1)
        {
            BgmVolume = PlayerPrefs.GetFloat("BGM_VOLUME", DefaultBgmVolume);
        }
        if (sfxVolume == -1)
        {
            SfxVolume = PlayerPrefs.GetFloat("SFX_VOLUME", DefaultSfxVolume);
        }
    }
}
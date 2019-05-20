using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixer mixer;
    public AudioSetting[] audioSettings;
    private enum AudioGroups { Master, Music, SFX };

    //public AudioClip[] musicClips;

#if UNITY_EDITOR
    public DefaultAsset[] musics;
    public DefaultAsset[] SFXs;
#endif

    public string[] musicNames;
    public string[] SFXNames;

    public AudioSource music;
    public AudioSource SFX;
    public AudioSource SFXPitch;

    public Button[] btns;

    public float sfxVolume = 0.5f;
    public float musicVolume = 0.5f;

    public string currentMainMusic = string.Empty;
    public string currentDeathMusic = string.Empty;
    public string[] playMusics;
    public string[] deathMusics;
    public string[] bossMusics;

#if UNITY_EDITOR
    Dictionary<string, AudioClip> musicClipDictionary = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> SFXClipDictionary = new Dictionary<string, AudioClip>();
#endif
    int currentMusicStreamID = -1;
    string currentMusicName = string.Empty;

    int currentSFXStreamID;
    Dictionary<string, int> SFXFileIDDictionary = new Dictionary<string, int>();

    void Awake()
    {
        instance = this;
        AndroidNativeAudio.makePool();
        //for (int i = 0; i < musicClips.Length; i++)
        //{
        //    musicClipDictionary.Add(musicClips[i].name, musicClips[i]);
        //}
#if UNITY_EDITOR || UNITY_STANDALONE
        for (int i = 0; i < musicNames.Length; i++)
        {
            StartCoroutine(LoadClipCoroutine("Music", musicNames[i], AudioType.OGGVORBIS, OnMusicLoadingCompleted));
        }
        for (int i = 0; i < SFXNames.Length; i++)
        {
            StartCoroutine(LoadClipCoroutine("SFX", SFXNames[i], AudioType.WAV, OnSFXLoadingCompleted));
        }
#else
        for (int i = 0; i < SFXNames.Length; i++)
        {
            SFXFileIDDictionary.Add(SFXNames[i], AndroidNativeAudio.load(string.Format("SFX/{0}.wav", SFXNames[i])));
        }
#endif

        //for (int i = 0; i < musicNames.Length; i++)
        //{
        //    musicFileIDDictionary.Add(musicNames[i], ANAMusic.load(string.Format("Music/{0}.ogg", musicNames[i]), false, false));
        //}

        BtnSound();
    }

#if UNITY_EDITOR || UNITY_STANDALONE
    IEnumerator LoadClipCoroutine(string folder, string name, AudioType type, System.Action<AudioClip> onLoadingCompleted)
    {
        string format = string.Empty;


        switch (type)
        {
            case AudioType.WAV:
                format = "wav";
                break;
            case AudioType.AIFF:
                format = "aif";
                break;
            case AudioType.OGGVORBIS:
                format = "ogg";
                break;
        }

        string file = (string.Format("{0}/{1}/{2}.{3}", Application.streamingAssetsPath, folder, name, format));


        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(file, type))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log("Sound Loading Fail:" + file);
            }
            else if (onLoadingCompleted != null)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                clip.name = name;
                onLoadingCompleted(clip);
            }
            www.Dispose();
        }
    }

    void OnSFXLoadingCompleted(AudioClip clip)
    {
        SFXClipDictionary.Add(clip.name, clip);
    }

    void OnMusicLoadingCompleted(AudioClip clip)
    {
        musicClipDictionary.Add(clip.name, clip);
    }

    [ContextMenu("SyncNames")]
    void SyncNames()
    {
        SFXNames = new string[SFXs.Length];
        for (int i = 0; i < SFXs.Length; i++)
        {
            SFXNames[i] = SFXs[i].name;
        }
        musicNames = new string[musics.Length];
        for (int i = 0; i < musics.Length; i++)
        {
            musicNames[i] = musics[i].name;
        }
    }
#endif

    void BtnSound()
    {
        for (int i = 0; i < btns.Length; i++)
        {
            btns[i].onClick.AddListener(() => PlaySFX("click"));
        }
    }

    void Start()
    {
        for (int i = 0; i < audioSettings.Length; i++)
        {
            audioSettings[i].Initialize();
        }
    }

    public void PlayRandomMusic()
    {
        PlayMusic(GetRandomMainMusic());
    }

    public string GetRandomMainMusic()
    {
        int rnd;
        string str;
        do
        {
            rnd = Random.Range(0, playMusics.Length);
            str = playMusics[rnd];
        } while (str.CompareTo(currentMainMusic) == 0);
        currentMainMusic = str;
        return str;
    }

    public string GetRandomDeathMusic()
    {
        int rnd = Random.Range(0, deathMusics.Length);
        string str = deathMusics[rnd];
        currentDeathMusic = str;
        return str;
    }

    public string GetRandomBossMusic()
    {
        int rnd = Random.Range(0, bossMusics.Length);
        string str = bossMusics[rnd];
        currentMainMusic = str;
        return str;
    }

    public void SetMasterVolume(float value)
    {
        audioSettings[(int)AudioGroups.Master].SetExposedParam(value);
    }

    public void SetMusicVolume(float value)
    {
        //if(value <= -20)
        //    audioSettings[(int)AudioGroups.Music].SetExposedParam(-80);
        //else
        //    audioSettings[(int)AudioGroups.Music].SetExposedParam(value);

#if UNITY_EDITOR || UNITY_STANDALONE
        audioSettings[(int)AudioGroups.Music].SetExposedParam(value);
#endif

        musicVolume = Mathf.InverseLerp(audioSettings[(int)AudioGroups.Music].slider.minValue,
    audioSettings[(int)AudioGroups.Music].slider.maxValue, value);
#if UNITY_EDITOR || UNITY_STANDALONE
        music.volume = musicVolume;
#else
        if(currentMusicStreamID != -1)
            ANAMusic.setVolume(currentMusicStreamID, musicVolume);
#endif
        PlayerPrefs.SetFloat(audioSettings[(int)AudioGroups.Music].exposedParam, value);
    }

    public void SetSFXVolume(float value)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        audioSettings[(int)AudioGroups.SFX].SetExposedParam(value);
#endif

        sfxVolume = Mathf.InverseLerp(audioSettings[(int)AudioGroups.SFX].slider.minValue,
    audioSettings[(int)AudioGroups.SFX].slider.maxValue, value) * SFX.volume;
        PlayerPrefs.SetFloat(audioSettings[(int)AudioGroups.SFX].exposedParam, value);

    }

    //public void Mute(int audio)
    //{
    //    audioSettings[audio].Mute();
    //}

    public void LoadMusic(string name)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        currentMusicName = name;
        ANAMusic.load(string.Format("Music/{0}.ogg", name));
#endif
    }

    void OnMusicLoaded(int ID)
    {
        currentMusicStreamID = ID;
        ANAMusic.setVolume(ID, musicVolume);
        ANAMusic.setLooping(ID, true);
    }

    public void PlayMusic(string name)
    {
        Debug.Log("PlayMusic:"+name);
#if UNITY_EDITOR || UNITY_STANDALONE
        if (music.clip && music.clip.name.CompareTo(name) == 0)
        {
            if (music.isPlaying)
                return;
            else
            {
                music.Play();
                return;
            }
        }

#elif !UNITY_EDITOR && UNITY_ANDROID
        if (currentMusicName.CompareTo(name) == 0)
        {
            if (currentMusicStreamID == -1 || ANAMusic.isPlaying(currentMusicStreamID))
                return;
            else
            {
                ANAMusic.play(currentMusicStreamID);
                return;
            }
        }
#endif

        //Debug.Log(string.Format("PlayMusic {0}", name));
        //if (musicClipDictionary.ContainsKey(name))
        //{
        //    music.clip = musicClipDictionary[name];
        //    music.Play();
        //}
        //else
        //{
        //    music.Stop();
        //    music.clip = null;
        //}

        StopMusic();

#if UNITY_EDITOR || UNITY_STANDALONE
        if (musicClipDictionary.ContainsKey(name))
        {
            music.clip = musicClipDictionary[name];
            music.Play();
        }
#endif
#if !UNITY_EDITOR && UNITY_ANDROID
        currentMusicName = name;
        currentMusicStreamID = ANAMusic.load(string.Format("Music/{0}.ogg", name), false, false, OnMusicLoadedPlay);
#endif
    }

    public void OnMusicLoadedPlay(int ID)
    {
        ANAMusic.setVolume(ID, musicVolume);
        ANAMusic.setLooping(ID, true);
        ANAMusic.play(ID);
    }

    public void StopMusic()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if(music.clip)
            music.Stop();

#else
        //    ANAMusic.pause(currentMusicStreamID);
        //    ANAMusic.seekTo(currentMusicStreamID, 0);
        if(currentMusicStreamID != -1)
        {
            ANAMusic.release(currentMusicStreamID);
            currentMusicStreamID = -1;
            currentMusicName = string.Empty;
        }
#endif
    }

    public void TogglePauseMusic()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (music.isPlaying)
            music.Pause();
        else
            music.UnPause();
#else
        if (currentMusicStreamID != -1)
        {
            if (ANAMusic.isPlaying(currentMusicStreamID))
                ANAMusic.pause(currentMusicStreamID);
            else
                ANAMusic.play(currentMusicStreamID);
        }
#endif
    }

    public void FadeInMusic(float time, System.Action onEnd = null)
    {
        StartCoroutine(FadeInMusicCorou(musicVolume, time, onEnd));
    }

    IEnumerator FadeInMusicCorou(float volume, float time, System.Action onEnd)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        music.volume = 0;
#else
        if (currentMusicStreamID == -1)
            yield break;
        else
            ANAMusic.setVolume(currentMusicStreamID, 0);
#endif

        float t = 0;
        if (time > 0)
        {
            do
            {
                yield return null;
                t += Time.unscaledDeltaTime / time;
#if UNITY_EDITOR || UNITY_STANDALONE
                music.volume = Mathf.Lerp(0, volume, t);
#else
            ANAMusic.setVolume(currentMusicStreamID, Mathf.Lerp(0, volume, t));
#endif
            } while (t < 1);
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        music.volume = volume;
#else
        ANAMusic.setVolume(currentMusicStreamID, volume);
#endif
        if (onEnd != null)
            onEnd.Invoke();
    }

    public void FadeOutMusic(float time, System.Action onEnd = null)
    {
        StartCoroutine(FadeOutMusicCorou(musicVolume, time, onEnd));
    }

    IEnumerator FadeOutMusicCorou(float volume, float time, System.Action onEnd)
    {
        float lastVolume = musicVolume;
#if !UNITY_EDITOR && UNITY_ANDROID
        if (currentMusicStreamID == -1)
            yield break;
#endif

        float t = 0;
        if (time > 0)
        {
            do
            {
                yield return null;
                t += Time.unscaledDeltaTime / time;
#if UNITY_EDITOR || UNITY_STANDALONE
                music.volume = Mathf.Lerp(lastVolume, 0, t);
#else
            ANAMusic.setVolume(currentMusicStreamID, Mathf.Lerp(lastVolume, 0, t));
#endif
            } while (t < 1);
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        music.volume = 0;
#else
        ANAMusic.setVolume(currentMusicStreamID, 0);
#endif
        if (onEnd != null)
            onEnd.Invoke();
    }

    //public IEnumerator PlaySound(AudioSource source, Transform parent)
    //{
    //    Transform sourceTransform = source.transform;
    //    sourceTransform.SetParent(null);
    //    source.Play();
    //    do
    //    {
    //        yield return null;
    //    } while (source.isPlaying);
    //    sourceTransform.SetParent(parent);
    //}

    public void PlaySFX(string name)
    {
        string[] str = name.Split(':');
        if(str.Length > 1)
        {
            float pitch;
            if (float.TryParse(str[1], out pitch))
            {
                //Debug.Log("SFX:" + str[0] + pitch);
                PlaySFX(str[0], pitch);
                return;
            }
            else
                Debug.LogErrorFormat("PlaySFX-Pitch Parsing False");
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        if (SFXClipDictionary.ContainsKey(name))
            SFX.PlayOneShot(SFXClipDictionary[name], sfxVolume);
#endif

#if !UNITY_EDITOR && UNITY_ANDROID
            currentSFXStreamID = AndroidNativeAudio.play(SFXFileIDDictionary[name], sfxVolume, -1);
#endif
    }

    public void PlaySFX(string name, float pitch)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (SFXClipDictionary.ContainsKey(name))
        {
            SFXPitch.pitch = pitch;
            SFXPitch.PlayOneShot(SFXClipDictionary[name], sfxVolume);
        }

#endif

#if !UNITY_EDITOR && UNITY_ANDROID
        try
        {
            currentSFXStreamID = AndroidNativeAudio.play(SFXFileIDDictionary[name], sfxVolume, -1, 1, 0, pitch);
        }catch
        {}
#endif
    }

    public void StopSFX()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        SFX.Stop();
#endif
#if !UNITY_EDITOR && UNITY_ANDROID
        AndroidNativeAudio.stop(currentSFXStreamID);
#endif
    }

#if !UNITY_EDITOR && UNITY_ANDROID
    void OnApplicationQuit()
    {
        // Clean up when done
        List<int> list = new List<int>(SFXFileIDDictionary.Values);
        for (int i = 0; i < list.Count; i++)
            AndroidNativeAudio.unload(list[i]);
        AndroidNativeAudio.releasePool();

        if(currentMusicStreamID != -1)
        {
            ANAMusic.release(currentMusicStreamID);
        }
    }
#endif
}

[System.Serializable]
public class AudioSetting
{
    public Slider slider;
    //public Button mute;
    public string exposedParam;

    public void Initialize()
    {
        slider.value = PlayerPrefs.GetFloat(exposedParam);
    }

    public void SetExposedParam(float value)
    {
        //if(value <= slider.minValue)
        //    mute.targetGraphic.color = mute.colors.disabledColor;
        //else
        //    mute.targetGraphic.color = mute.colors.normalColor;
        AudioManager.instance.mixer.SetFloat(exposedParam, value);
        PlayerPrefs.SetFloat(exposedParam, value);
    }

    //public void Mute()
    //{
    //    float value = slider.value;
    //    if (slider.value > slider.minValue)
    //    {
    //        value = slider.value;
    //        slider.value = slider.minValue;
    //        PlayerPrefs.SetFloat(exposedParam, value);
    //    }
    //    else
    //    {
    //        value = slider.value = PlayerPrefs.GetFloat(exposedParam);
    //    }
    //}
}
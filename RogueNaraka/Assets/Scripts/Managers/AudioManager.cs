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

    public AudioClip[] musicClips;

#if UNITY_EDITOR
    public DefaultAsset[] SFXs;
#endif

    public string[] SFXNames;

    public AudioSource music;
    public AudioSource SFX;

    public Button[] btns;

    public float sfxVolume = 0.5f;

    public string currentMainMusic;
    public string currentDeathMusic;
    public string[] playMusics;
    public string[] deathMusics;
    public string[] bossMusics;

    Dictionary<string, AudioClip> musicClipDictionary = new Dictionary<string, AudioClip>();
#if UNITY_EDITOR
    Dictionary<string, AudioClip> SFXClipDictionary = new Dictionary<string, AudioClip>();
#endif
#if !UNITY_EDITOR && UNITY_ANDROID
    Dictionary<string, int> fileIDDictionary = new Dictionary<string, int>();
#endif

    void Awake()
    {
        instance = this;
        AndroidNativeAudio.makePool();
        for (int i = 0; i < musicClips.Length; i++)
        {
            musicClipDictionary.Add(musicClips[i].name, musicClips[i]);
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        for (int i = 0; i < SFXNames.Length; i++)
        {
            StartCoroutine(LoadClipCoroutine(SFXNames[i], OnAudioClipLoadingCompleted));
        }
#endif

#if !UNITY_EDITOR && UNITY_ANDROID
        for (int i = 0; i < SFXNames.Length; i++)
        {
            fileIDDictionary.Add(SFXNames[i], AndroidNativeAudio.load(string.Format("SFX/{0}.wav", SFXNames[i])));
        }
#endif

        BtnSound();
    }

    #if UNITY_EDITOR || UNITY_STANDALONE
    IEnumerator LoadClipCoroutine(string name, System.Action<AudioClip> onLoadingCompleted)
    {
        string file = (string.Format("{0}/SFX/{1}.wav", Application.streamingAssetsPath, name));
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(file, AudioType.WAV))
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

    void OnAudioClipLoadingCompleted(AudioClip clip)
    {
        SFXClipDictionary.Add(clip.name, clip);
    }

    [ContextMenu("Temp")]
    void Temp()
    {
        SFXNames = new string[SFXs.Length];
        for (int i = 0; i < SFXs.Length; i++)
        {
            SFXNames[i] = SFXs[i].name;
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
        if(value <= -20)
            audioSettings[(int)AudioGroups.Music].SetExposedParam(-80);
        else
            audioSettings[(int)AudioGroups.Music].SetExposedParam(value);
    }

    public void SetSFXVolume(float value)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        audioSettings[(int)AudioGroups.SFX].SetExposedParam(value);
#endif

#if !UNITY_EDITOR && UNITY_ANDROID
                sfxVolume = Mathf.InverseLerp(audioSettings[(int)AudioGroups.SFX].slider.minValue,
            audioSettings[(int)AudioGroups.SFX].slider.maxValue, value) * SFX.volume;
        PlayerPrefs.SetFloat(audioSettings[(int)AudioGroups.SFX].exposedParam, value);
#endif

    }

    //public void Mute(int audio)
    //{
    //    audioSettings[audio].Mute();
    //}

    public void PlayMusic(string name)
    {
        if (music.clip && music.clip.name.CompareTo(name) == 0)
            return;

        Debug.Log(string.Format("PlayMusic {0}", name));
        if (musicClipDictionary.ContainsKey(name))
        {
            music.clip = musicClipDictionary[name];
            music.Play();
        }
        else
        {
            music.Stop();
            music.clip = null;
        }
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
#if UNITY_EDITOR || UNITY_STANDALONE
        if (SFXClipDictionary.ContainsKey(name))
            SFX.PlayOneShot(SFXClipDictionary[name]);
#endif

#if !UNITY_EDITOR && UNITY_ANDROID
        try
        {
            AndroidNativeAudio.play(fileIDDictionary[name], sfxVolume);
        }catch
        {}
#endif
    }
#if !UNITY_EDITOR && UNITY_ANDROID
    void OnApplicationQuit()
    {
        // Clean up when done
        List<int> list = new List<int>(fileIDDictionary.Values);
        for (int i = 0; i < list.Count; i++)
            AndroidNativeAudio.unload(list[i]);
        AndroidNativeAudio.releasePool();
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
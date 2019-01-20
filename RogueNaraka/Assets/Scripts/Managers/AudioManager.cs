using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixer mixer;
    public AudioSetting[] audioSettings;
    private enum AudioGroups { Master, Music, SFX };

    public AudioClip[] musicClips;
    public AudioClip[] SFXClips;

    public AudioSource music;
    public AudioSource SFX;

    public Button[] btns;
    Dictionary<string, AudioClip> musicClipDictionary = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> SFXClipDictionary = new Dictionary<string, AudioClip>();

    void Awake()
    {
        instance = this;
        for (int i = 0; i < musicClips.Length; i++)
        {
            musicClipDictionary.Add(musicClips[i].name, musicClips[i]);
        }
        for (int i = 0; i < SFXClips.Length; i++)
        {
            SFXClipDictionary.Add(SFXClips[i].name, SFXClips[i]);
        }
        BtnSound();
    }


    [ContextMenu("BtnSound")]
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

    public void SetMasterVolume(float value)
    {
        audioSettings[(int)AudioGroups.Master].SetExposedParam(value);
    }

    public void SetMusicVolume(float value)
    {
        audioSettings[(int)AudioGroups.Music].SetExposedParam(value);
    }

    public void SetSFXVolume(float value)
    {
        audioSettings[(int)AudioGroups.SFX].SetExposedParam(value);
    }

    //public void Mute(int audio)
    //{
    //    audioSettings[audio].Mute();
    //}

    public void PlayMusic(string name)
    {
        if (music.clip && music.clip.name.CompareTo(name) == 0)
            return;

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
        if(SFXClipDictionary.ContainsKey(name))
            SFX.PlayOneShot(SFXClipDictionary[name]);
    }
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
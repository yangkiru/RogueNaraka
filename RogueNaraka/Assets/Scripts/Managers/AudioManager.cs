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

    public AudioSource[] musics;
    public AudioSource currentMusic;
    Dictionary<string, AudioSource> sources = new Dictionary<string, AudioSource>();

    void Awake()
    {
        instance = this;
        for (int i = 0; i < musics.Length; i++)
        {
            sources.Add(musics[i].clip.name, musics[i]);
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

    public void Mute(int audio)
    {
        audioSettings[audio].Mute();
    }

    public void PlayMusic(string name)
    {
        try
        {
            if (currentMusic && currentMusic.clip.name.CompareTo(name) != 0)
                currentMusic.Stop();
            else if (currentMusic)
                return;
            currentMusic = sources[name];
            currentMusic.Play();
        }
        catch
        {
            Debug.LogError(string.Format("Can't find {0}", name));
        }
    }
}

[System.Serializable]
public class AudioSetting
{
    public Slider slider;
    public Button mute;
    public string exposedParam;

    public void Initialize()
    {
        slider.value = PlayerPrefs.GetFloat(exposedParam);
    }

    public void SetExposedParam(float value)
    {
        if(value <= slider.minValue)
            mute.targetGraphic.color = mute.colors.disabledColor;
        else
            mute.targetGraphic.color = mute.colors.normalColor;
        AudioManager.instance.mixer.SetFloat(exposedParam, value);
        PlayerPrefs.SetFloat(exposedParam, value);
    }

    public void Mute()
    {
        float value = slider.value;
        if (slider.value > slider.minValue)
        {
            value = slider.value;
            slider.value = slider.minValue;
            PlayerPrefs.SetFloat(exposedParam, value);
        }
        else
        {
            value = slider.value = PlayerPrefs.GetFloat(exposedParam);
        }
    }
}
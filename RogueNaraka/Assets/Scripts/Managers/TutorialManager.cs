using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public TutorialText[] startTexts;
    public bool[] isTutorial;
    public bool isPause { get; set;}

    public bool isPlaying;
    public GameObject pausePnl;
    public GameObject settingPnl;

    public static TutorialManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        isTutorial = new bool[startTexts.Length];
        for(int i = 0; i < startTexts.Length; i++)
        {
            isTutorial[i] = PlayerPrefs.GetInt(string.Format("isTutorial{0}", i)) == 0;
        }

        StartCoroutine(Intro());
    }

    IEnumerator Intro()
    {
        float t = 1.5f;
        do
        {
            yield return null;
            t -= Time.unscaledDeltaTime;
        } while (t > 0);
        StartTutorial(0);
    }
    public void StartTutorial(int i)
    {
        if (isTutorial[i])
        {
            startTexts[i].TextOn();
            isPlaying = true;
        }
    }

    public void EndTutorial(int i)
    {
        isTutorial[i] = false;
        PlayerPrefs.SetInt(string.Format("isTutorial{0}", i), 1);
        isPlaying = false;
    }

    public void ResetTutorial()
    {
        for (int i = 0; i < startTexts.Length; i++)
        {
            PlayerPrefs.SetInt(string.Format("isTutorial{0}", i), 0);
        }
    }
}

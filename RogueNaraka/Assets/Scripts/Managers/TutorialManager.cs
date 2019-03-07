#define TUTORIAL
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
        isTutorial = new bool[startTexts.Length];
        for (int i = 0; i < startTexts.Length; i++)
        {
            isTutorial[i] = PlayerPrefs.GetInt(string.Format("isTutorial{0}", i)) == 0;
        }
    }

    private void Start()
    {

        //isTutorial = new bool[startTexts.Length];
        //for (int i = 0; i < startTexts.Length; i++)
        //{
        //    isTutorial[i] = PlayerPrefs.GetInt(string.Format("isTutorial{0}", i)) == 0;
        //}
        StartCoroutine(Intro());
    }

    IEnumerator Intro()
    {
        float t = 0.5f;
        do
        {
            yield return null;
            t -= Time.unscaledDeltaTime;
        } while (t > 0);
        instance = this;
        isTutorial = new bool[startTexts.Length];
        for (int i = 0; i < startTexts.Length; i++)
        {
            isTutorial[i] = PlayerPrefs.GetInt(string.Format("isTutorial{0}", i)) == 0;
        }
        yield return null;
        StartTutorial(0);
    }


    public void StartTutorial(int i)
    {
#if TUTORIAL
        if (isTutorial[i])
        {
            startTexts[i].TextOn();
            isPlaying = true;
        }
#endif
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

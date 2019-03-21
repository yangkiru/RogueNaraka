using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public TutorialText[] startTexts;
    public List<bool> isTutorial = new List<bool>();
    public bool isPause { get; set;}

    public bool isPlaying;
    public GameObject pausePnl;
    public GameObject settingPnl;

    public static TutorialManager instance;

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < startTexts.Length; i++)
        {
            isTutorial.Add(PlayerPrefs.GetInt(string.Format("isTutorial{0}", i)) == 0);
        }
        
    }

    //IEnumerator IntroCorou()
    //{
    //    yield return null;
    //    StartTutorial(0);
    //}

    private void Start()
    {
        //StartCoroutine(IntroCorou());
        //    //isTutorial = new bool[startTexts.Length];
        //    //for (int i = 0; i < startTexts.Length; i++)
        //    //{
        //    //    isTutorial[i] = PlayerPrefs.GetInt(string.Format("isTutorial{0}", i)) == 0;
        //    //}
    }

    public void ResetTutorial()
    {
        for (int i = 0; i < startTexts.Length; i++)
        {
            isTutorial[i] = true;
            PlayerPrefs.SetInt(string.Format("isTutorial{0}", i), 0);
        }
    }


    bool isPauseBtn;

    public void StartTutorial(int i)
    {
        if (isTutorial[i])
        {
            //Debug.Log("StartTutorial" + i + ":" + PlayerPrefs.GetInt(string.Format("isTutorial{0}", i)));
            startTexts[i].TextOn();
            isPlaying = true;
            if (GameManager.instance.pauseBtn.activeSelf)
                isPauseBtn = true;
            GameManager.instance.SetSettingBtn(true);
        }
    }

    public void EndTutorial(int i)
    {
        isTutorial[i] = false;
        PlayerPrefs.SetInt(string.Format("isTutorial{0}", i), 1);
        //Debug.Log("EndTutorial" + i + ":" + PlayerPrefs.GetInt(string.Format("isTutorial{0}", i)));
        isPlaying = false;
        if(isPauseBtn)
            GameManager.instance.SetSettingBtn(false);
    }

    [ContextMenu("CheckTutorial")]
    public void CheckTutorial()
    {
        int i = 0;
        Debug.Log("CheckTutorial" + i + ":" + PlayerPrefs.GetInt(string.Format("isTutorial{0}", i)));
    }
}

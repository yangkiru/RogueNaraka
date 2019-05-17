using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RogueNaraka.Escapeable;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public TutorialText[] startTexts;
    public List<bool> isTutorial = new List<bool>();
    public bool isPause { get; set;}

    public bool isPlaying;
    public GameObject pausePnl;
    public GameObject settingPnl;

    public Button skipBtn;

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

    public void SkipTutorial()
    {
        for (int i = 0; i < startTexts.Length; i++)
        {
            GameObject parent = startTexts[i].transform.parent.gameObject;
            if(parent.activeSelf)
            {
                parent.GetComponent<Escapeable>().OnEscape();
                return;
            }
        }
        
    }

    bool isPauseBtn;

    public void StartTutorial(int i)
    {
        if (isTutorial[i])
        {
            skipBtn.gameObject.SetActive(true);
            isPauseBtn = GameManager.instance.pauseBtn.activeSelf;
            GameManager.instance.SetSettingBtn(true);
            //Debug.Log("StartTutorial" + i + ":" + PlayerPrefs.GetInt(string.Format("isTutorial{0}", i)));
            startTexts[i].TextOn();
            isPlaying = true;
        }
    }

    public void EndTutorial(int i)
    {
        skipBtn.gameObject.SetActive(false);
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

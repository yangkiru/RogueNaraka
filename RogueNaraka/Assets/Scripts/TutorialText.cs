using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using TMPro;
public class TutorialText : MonoBehaviour
{
    [TextArea]
    public string[] texts;
    public TextMeshProUGUI tmpro;
    public TutorialText next;

    public TutorialEvent onStart;
    public TutorialEvent onEnd;

    float delay = 0.025f;

    private void Reset()
    {
        tmpro = GetComponent<TextMeshProUGUI>();
    }

    public void TextOn()
    {
        int lang = (int)GameManager.language;
        string txt = texts?[lang];
        if (txt == null)
            txt = texts[0];
        if (onStart != null)
            onStart.Invoke();
        gameObject.SetActive(true);
        StartCoroutine(TextTyping(txt));
    }

    //IEnumerator TextAppear()
    //{
    //    Language currentLang = GameManager.language;
    //    string current = string.Empty;
    //    int lineCount = tmpro.textInfo.lineCount;
    //    int wordCount = tmpro.textInfo.lineInfo[0].wordCount;

    //    for(int i = 0; i < lineCount; i++)
    //    {
    //        string current =  tmpro.textInfo.lineInfo[i].
    //    }

    //}

    IEnumerator TextTyping(string text)
    {
        Language currentLang = GameManager.language;
        string current = string.Empty;
        tmpro.text = current;
        
        for (int i = 0; i < text.Length; i++)
        {
            if(currentLang != GameManager.language)
            {
                currentLang = GameManager.language;
                i = -1;
                text = texts?[(int)currentLang];
                current = string.Empty;
                continue;
            }
            float t = delay;

            //Physics2D.Raycast(GameManager.GetMousePosition())
            
            do
            {
                yield return null;
                if(!TutorialManager.instance.isPause)
                    t -= Time.unscaledDeltaTime;
            } while (t > 0);
            if (Input.anyKey && !TutorialManager.instance.isPause && i + 1 < text.Length)
            {
                current = string.Format("{0}{1}{2}", current, text[i], text[i + 1]);
                i++;
            }
            else
                current = string.Format("{0}{1}", current, text[i]);
            tmpro.text = current;
        }
        do
        {
            yield return null;
        } while (!Input.anyKeyDown || TutorialManager.instance.isPause);
        if (onEnd != null)
            onEnd.Invoke();
        if (next)
            next.TextOn();
        gameObject.SetActive(false);
    }
    [Serializable]
    public class TutorialEvent : UnityEvent { }
}

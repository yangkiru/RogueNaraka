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

    float delay = 0.05f;

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

    IEnumerator TextTyping(string text)
    {
        Language currentLang = GameManager.language;
        string font = GameDatabase.instance.langFonts[(int)currentLang];
        string current = font == string.Empty ? string.Empty : string.Format("<font=\"{0}\">", font);
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
            if (Input.anyKey && !TutorialManager.instance.isPause)
                t = delay * 0.25f;
            do
            {
                yield return null;
                if(!TutorialManager.instance.isPause)
                    t -= Time.unscaledDeltaTime;
            } while (t > 0);
            current = string.Format("{0}{1}", current, text[i]);
            tmpro.text = current;
        }
        do
        {
            yield return null;
        } while (!Input.anyKeyDown || TutorialManager.instance.isPause);

        gameObject.SetActive(false);
        if (onEnd != null)
            onEnd.Invoke();
        if (next)
            next.TextOn();
    }
    [Serializable]
    public class TutorialEvent : UnityEvent { }
}

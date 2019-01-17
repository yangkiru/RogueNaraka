using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance;
    public Image pnl;

    private void Awake()
    {
        instance = this;
    }
    public void FadeOut(float t = 1, bool ignoreTimeScale = false, FadeEvent onEnd = null)
    {
        Debug.Log("FadeOut");
        StartCoroutine(FadeOutCorou(t, ignoreTimeScale, onEnd));
    }

    IEnumerator FadeOutCorou(float t = 1, bool ignoreTimeScale = false, FadeEvent onEnd = null)
    {
        Color color = pnl.color;
        color.a = 0;
        pnl.color = color;
        pnl.gameObject.SetActive(true);
        float tt = t;
        do
        {
            yield return null;
            float time = ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            t -= time;
            color.a += time / tt;
            pnl.color = color;
        } while (t > 0);
        if (onEnd != null)
            onEnd.Invoke();
    }
    public void FadeIn(float t = 1, bool ignoreTimeScale = false, FadeEvent onEnd = null)
    {
        Debug.Log("FadeIn");
        StartCoroutine(FadeInCorou(t, ignoreTimeScale, onEnd));
    }

    IEnumerator FadeInCorou(float t = 1, bool ignoreTimeScale = false, FadeEvent onEnd = null)
    {
        Color color = pnl.color;
        color.a = 1;
        pnl.color = color;
        pnl.gameObject.SetActive(true);
        float tt = t;
        do
        {
            yield return null;
            float time = ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            t -= time;
            color.a -= time / tt;
            pnl.color = color;
        } while (t > 0);
        pnl.gameObject.SetActive(false);
        if (onEnd != null)
            onEnd.Invoke();
    }

    [Serializable]
    public class FadeEvent : UnityEvent { }
}

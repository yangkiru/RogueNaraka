using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public float FadeIntime = 1;
    public float FadeOuttime = 1;
    public bool ignoreTimeScaleIn;
    public bool ignoreTimeScaleOut;
    public bool isClickableIn;
    public bool isClickableOut;
    public FadeManager.FadeEvent onFadeInEnd;
    public FadeManager.FadeEvent onFadeOutEnd;

    public void FadeIn()
    {
        FadeManager.instance.FadeIn(FadeIntime, ignoreTimeScaleIn, isClickableIn, onFadeInEnd);
    }

    public void FadeOut()
    {
        FadeManager.instance.FadeOut(FadeOuttime, ignoreTimeScaleOut, isClickableIn, onFadeOutEnd);
    }
}

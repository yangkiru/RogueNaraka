﻿using UnityEngine;
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
    public FadeManager.FadeEvent onFadeInEnd;
    public FadeManager.FadeEvent onFadeOutEnd;

    public void FadeIn()
    {
        Debug.Log("FadeIn");
        FadeManager.instance.FadeIn(FadeIntime, ignoreTimeScaleIn, onFadeInEnd);
    }

    public void FadeOut()
    {
        Debug.Log("FadeOut");
        FadeManager.instance.FadeOut(FadeOuttime, ignoreTimeScaleOut, onFadeOutEnd);
    }
}

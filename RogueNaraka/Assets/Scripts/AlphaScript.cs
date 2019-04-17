using RogueNaraka.TimeScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AlphaScript : MonoBehaviour
{
    public GameObject target;
    Image mainImage;
    TMPro.TextMeshProUGUI mainTxt;
    public Image[] images;
    public TMPro.TextMeshProUGUI[] txts;
    public ParticleSystem[] particles;

    public bool isAlphaDownOnEnable;
    public float alphaDownDelay;
    public float alphaDownTime;

    float originAlpha;
    float currentAlpha;
    float lastAlpha = -1;

    public AlphaEvent OnAlphaZero { get { return onAlphaZero; } set { onAlphaZero = value; } }

    [SerializeField]
    AlphaEvent onAlphaZero;
    float leftTime;

    List<float> alphas = new List<float>();

    enum STATE { NOT_READY, READY, DOWN }
    STATE currentState;
    STATE lastState;

    private void Reset()
    {
        mainImage = GetComponent<Image>();
        mainTxt = GetComponent<TMPro.TextMeshProUGUI>();
        target = mainImage ? mainImage.gameObject : mainTxt.gameObject;
    }

    private void OnEnable()
    {
        currentState = isAlphaDownOnEnable ? STATE.READY : STATE.NOT_READY;
    }

    private void Start()
    {
        mainImage = target.GetComponent<Image>();
        mainTxt = target.GetComponent<TMPro.TextMeshProUGUI>();
        originAlpha = mainImage ? mainImage.color.a : mainTxt ? mainTxt.color.a : 1;
        for (int i = 0; i < images.Length; i++)
        {
            alphas.Add(images[i].color.a);
        }
        for (int i = 0; i < txts.Length; i++)
            alphas.Add(txts[i].color.a);
        for (int i = 0; i < particles.Length; i++)
            alphas.Add(particles[i].main.startColor.color.a);
    }

    private void FixedUpdate()
    {
        switch(currentState)
        {
            case STATE.NOT_READY:
                break;
            case STATE.READY:
                if (currentState != lastState)
                {
                    leftTime = alphaDownDelay;
                    lastState = currentState;
                }
                leftTime -= TimeManager.Instance.FixedDeltaTime;
                if (leftTime <= 0)
                    currentState = STATE.DOWN;
                break;
            case STATE.DOWN:
                if (currentState != lastState)
                {
                    leftTime = alphaDownTime;
                    lastState = currentState;
                }
                leftTime -= TimeManager.Instance.FixedDeltaTime;
                Color color = mainImage ? mainImage.color : mainTxt ? mainTxt.color : Color.clear;
                if (alphaDownTime == 0)
                    color.a = 0;
                else
                    color.a = Mathf.Lerp(0, originAlpha, leftTime / alphaDownTime);
                if (mainImage)
                    mainImage.color = color;
                else if (mainTxt)
                    mainTxt.color = color;
                else
                    Debug.LogError("AlphaScript:Doesn't have Image or Text.");
                if (leftTime <= 0)
                {
                    if (onAlphaZero != null)
                        onAlphaZero.Invoke();
                    currentState = STATE.NOT_READY;
                    target.SetActive(false);
                    color.a = originAlpha;
                    if (mainImage)
                        mainImage.color = color;
                    else if (mainTxt)
                        mainTxt.color = color;
                }
                break;
        }

        currentAlpha = mainImage ? mainImage.color.a : mainTxt ? mainTxt.color.a : 1;

        if (lastAlpha != currentAlpha)
        {
            float value = Mathf.InverseLerp(0, originAlpha, currentAlpha);
            for(int i = 0; i < images.Length; i++)
            {
                Color color = images[i].color;
                color.a = alphas[i] * value;
                images[i].color = color;
            }
            for (int i = 0; i < txts.Length; i++)
            {
                Color color = txts[i].color;
                color.a = alphas[images.Length + i] * value;
                txts[i].color = color;
            }
            for (int i = 0; i < particles.Length; i++)
            {
                Color color = particles[i].main.startColor.color;
                color.a = alphas[images.Length + txts.Length + i] * value;
                var main = particles[i].main;
                var startColor = main.startColor;
                startColor.color = color;
                main.startColor = startColor;
            }
            lastAlpha = currentAlpha;
        }
    }

    [Serializable]
    public class AlphaEvent : UnityEvent { }
}

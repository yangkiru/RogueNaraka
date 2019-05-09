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
    public bool isUnsclaedTime;
    public float alphaDownDelay;
    public float alphaDownTime;

    //float originAlpha;
    //float currentAlpha;
    //float lastAlpha = -1;

    public AlphaEvent OnAlphaZero { get { return onAlphaZero; } set { onAlphaZero = value; } }

    [SerializeField]
    AlphaEvent onAlphaZero;
    //float leftTime;

    //enum STATE { NOT_READY, READY, DOWN }
    //STATE currentState;
    //STATE lastState;

    private void Reset()
    {
        mainImage = GetComponent<Image>();
        mainTxt = GetComponent<TMPro.TextMeshProUGUI>();
        target = mainImage ? mainImage.gameObject : mainTxt.gameObject;
    }

    private void OnEnable()
    {
        //currentState = isAlphaDownOnEnable ? STATE.READY : STATE.NOT_READY;
        if(isAlphaDownOnEnable)
            AlphaDown();
    }

    private void AlphaDown()
    {
        StartCoroutine(AlphaDownCorou());
    }

    enum MAIN_TYPE
    {
        IMAGE, TXT, NULL
    }

    IEnumerator AlphaDownCorou()
    {
        //Init
        List<float> alphas = new List<float>();

        mainImage = target.GetComponent<Image>();
        mainTxt = target.GetComponent<TMPro.TextMeshProUGUI>();

        MAIN_TYPE isMainImage = mainImage ? MAIN_TYPE.IMAGE : mainTxt ? MAIN_TYPE.TXT : MAIN_TYPE.NULL;
        Color color = isMainImage == MAIN_TYPE.IMAGE ? mainImage.color : isMainImage == MAIN_TYPE.TXT ? mainTxt.color : Color.white;
        float originAlpha = isMainImage == MAIN_TYPE.IMAGE ? mainImage.color.a : isMainImage == MAIN_TYPE.TXT ? mainTxt.color.a : 1;
        
        for (int i = 0; i < images.Length; i++)
            alphas.Add(images[i].color.a);
        for (int i = 0; i < txts.Length; i++)
            alphas.Add(txts[i].color.a);
        for (int i = 0; i < particles.Length; i++)
            alphas.Add(particles[i].main.startColor.color.a);

        float t = 0;
        float reverse = 1 / alphaDownTime;
        do
        {
            yield return null;
            t += reverse * (isUnsclaedTime ? TimeManager.Instance.UnscaledDeltaTime : TimeManager.Instance.DeltaTime);
            //ColorDown For Slave
            color.a = Mathf.Lerp(originAlpha, 0, t);

            switch(isMainImage)
            {
                case MAIN_TYPE.IMAGE: mainImage.color = color; break;
                case MAIN_TYPE.TXT: mainTxt.color = color; break;
            }
            
            for (int i = 0; i < images.Length; i++)
            {
                images[i].color = new Color(images[i].color.r, images[i].color.g, images[i].color.b, Mathf.Lerp(alphas[i], 0, t));
            }
            for (int i = 0; i < txts.Length; i++)
            {
                txts[i].color = new Color(txts[i].color.r, txts[i].color.g, txts[i].color.b, Mathf.Lerp(alphas[images.Length + i], 0, t));
            }
            for (int i = 0; i < particles.Length; i++)
            {
                var main = particles[i].main;
                var startColor = main.startColor;
                startColor.color = new Color(particles[i].main.startColor.color.r, particles[i].main.startColor.color.g, particles[i].main.startColor.color.b, Mathf.Lerp(alphas[images.Length + txts.Length + i], 0, t));
                main.startColor = startColor;
            }
        } while (t < 1);

        //Reset
        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = new Color(images[i].color.r, images[i].color.g, images[i].color.b, alphas[i]);
        }
        for (int i = 0; i < txts.Length; i++)
        {
            txts[i].color = new Color(txts[i].color.r, txts[i].color.g, txts[i].color.b, alphas[images.Length + i]);
        }
        for (int i = 0; i < particles.Length; i++)
        {
            var main = particles[i].main;
            var startColor = main.startColor;
            startColor.color = new Color(particles[i].main.startColor.color.r, particles[i].main.startColor.color.g, particles[i].main.startColor.color.b, alphas[images.Length + txts.Length + i]);
            main.startColor = startColor;
        }

        color.a = originAlpha;
        switch(isMainImage)
        {
            case MAIN_TYPE.IMAGE: mainImage.color = color; break;
            case MAIN_TYPE.TXT: mainTxt.color = color; break;
        }            

        //End
        if (onAlphaZero != null)
            onAlphaZero.Invoke();
       
        target.SetActive(false);
    }

    //private void Start()
    //{
    //    mainImage = target.GetComponent<Image>();
    //    mainTxt = target.GetComponent<TMPro.TextMeshProUGUI>();
    //    originAlpha = mainImage ? mainImage.color.a : mainTxt ? mainTxt.color.a : 1;
    //    for (int i = 0; i < images.Length; i++)
    //    {
    //        alphas.Add(images[i].color.a);
    //    }
    //    for (int i = 0; i < txts.Length; i++)
    //        alphas.Add(txts[i].color.a);
    //    for (int i = 0; i < particles.Length; i++)
    //        alphas.Add(particles[i].main.startColor.color.a);
    //}

    //private void Update()
    //{
    //    switch (currentState)
    //    {
    //        case STATE.NOT_READY:
    //            break;
    //        case STATE.READY:
    //            if (currentState != lastState)
    //            {
    //                leftTime = alphaDownDelay;
    //                lastState = currentState;
    //            }
    //            leftTime -= isUnsclaedTime ? TimeManager.Instance.UnscaledDeltaTime : TimeManager.Instance.DeltaTime;
    //            if (leftTime <= 0)
    //                currentState = STATE.DOWN;
    //            break;
    //        case STATE.DOWN:
    //            if (currentState != lastState)
    //            {
    //                leftTime = alphaDownTime;
    //                lastState = currentState;
    //            }
    //            leftTime -= isUnsclaedTime ? TimeManager.Instance.UnscaledDeltaTime : TimeManager.Instance.DeltaTime;
    //            Color color = mainImage ? mainImage.color : mainTxt ? mainTxt.color : Color.clear;
    //            if (alphaDownTime == 0)
    //                color.a = 0;
    //            else
    //                color.a = Mathf.Lerp(0, originAlpha, leftTime / alphaDownTime);
    //            if (mainImage)
    //                mainImage.color = color;
    //            else if (mainTxt)
    //                mainTxt.color = color;
    //            else
    //                Debug.LogError("AlphaScript:Doesn't have Image or Text.");
    //            if (leftTime <= 0)
    //            {
    //                if (onAlphaZero != null)
    //                    onAlphaZero.Invoke();
    //                currentState = STATE.NOT_READY;
    //                target.SetActive(false);
    //                color.a = originAlpha;
    //                if (mainImage)
    //                    mainImage.color = color;
    //                else if (mainTxt)
    //                    mainTxt.color = color;
    //            }

    //            float value = Mathf.InverseLerp(0, originAlpha, color.a);
    //            for (int i = 0; i < images.Length; i++)
    //            {
    //                color = images[i].color;
    //                color.a = alphas[i] * value;
    //                images[i].color = color;
    //            }
    //            for (int i = 0; i < txts.Length; i++)
    //            {
    //                color = txts[i].color;
    //                color.a = alphas[images.Length + i] * value;
    //                txts[i].color = color;
    //            }
    //            for (int i = 0; i < particles.Length; i++)
    //            {
    //                color = particles[i].main.startColor.color;
    //                color.a = alphas[images.Length + txts.Length + i] * value;
    //                var main = particles[i].main;
    //                var startColor = main.startColor;
    //                startColor.color = color;
    //                main.startColor = startColor;
    //            }
    //            break;
    //    }
    //}

    [Serializable]
    public class AlphaEvent : UnityEvent { }
}

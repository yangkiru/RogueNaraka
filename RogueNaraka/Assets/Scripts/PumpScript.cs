using UnityEngine;
using System.Collections;
using RogueNaraka.TimeScripts;
using UnityEngine.Events;
using System;

public class PumpScript : MonoBehaviour
{
    public Vector3 pumpSize;

    public float upTime;
    public float downTime;

    public bool isUnsclaedTime;
    public bool isPumpOnEnable;

    public Transform cachedTransform;

    public PumpEvent OnPumpStart { get { return onPumpStart; } set { onPumpStart = value; } }

    [SerializeField]
    PumpEvent onPumpStart;

    public PumpEvent OnPumpEnd { get { return onPumpEnd; } set { onPumpEnd = value; } }

    [SerializeField]
    PumpEvent onPumpEnd;

    //enum STATE { NONE, UP, DOWN }

    //STATE currentState;
    //STATE lastState;

    //Vector3 originalSize;

    float leftTime;

    private void Reset()
    {
        cachedTransform = GetComponent<Transform>();
    }

    //private void Start()
    //{
    //    originalSize = cachedTransform.localScale;
    //}

    private void OnEnable()
    {
        if (isPumpOnEnable)
            Pump();
    }

    public void Pump()
    {
        //currentState = STATE.UP;
        StartCoroutine(PumpCorou());
    }

    IEnumerator PumpCorou()
    {
        if (onPumpStart != null)
            onPumpStart.Invoke();

        Vector3 originalSize = cachedTransform.localScale;
        float t = 0;
        float reverse = 1 / upTime;
        do
        {
            yield return null;
            t += reverse * (isUnsclaedTime ? TimeManager.Instance.UnscaledDeltaTime : TimeManager.Instance.DeltaTime);
            cachedTransform.localScale = Vector3.Lerp(originalSize, pumpSize, t);
        } while (t < 1);

        cachedTransform.localScale = pumpSize;

        t = 0;
        reverse = 1 / downTime;

        do
        {
            yield return null;
            t += reverse * (isUnsclaedTime ? TimeManager.Instance.UnscaledDeltaTime : TimeManager.Instance.DeltaTime);
            cachedTransform.localScale = Vector3.Lerp(pumpSize, originalSize, t);
        } while (t < 1);

        cachedTransform.localScale = originalSize;

        if (onPumpEnd != null)
            onPumpEnd.Invoke();
    }

    //private void Update()
    //{
    //    switch (currentState)
    //    {
    //        case STATE.NONE:
    //            break;
    //        case STATE.UP:
    //            {
    //                if (currentState != lastState)
    //                {
    //                    leftTime = upTime;
    //                    lastState = currentState;
    //                }
    //                leftTime -= isUnsclaedTime ? TimeManager.Instance.UnscaledDeltaTime : TimeManager.Instance.DeltaTime;
    //                Vector3 size;
    //                if (leftTime <= 0)
    //                {
    //                    currentState = STATE.DOWN;
    //                    leftTime = 0;
    //                }
    //                if (upTime == 0)
    //                    size = pumpSize;
    //                else
    //                    size = Vector3.Lerp(pumpSize, originalSize, leftTime / upTime);
    //                cachedTransform.localScale = size;
    //            }
    //            break;
    //        case STATE.DOWN:
    //            {
    //                if (currentState != lastState)
    //                {
    //                    leftTime = downTime;
    //                    lastState = currentState;
    //                }
    //                leftTime -= isUnsclaedTime ? TimeManager.Instance.UnscaledDeltaTime : TimeManager.Instance.DeltaTime;
    //                Vector3 size;
    //                if (leftTime <= 0)
    //                {
    //                    currentState = STATE.NONE;
    //                    leftTime = 0;
    //                }
    //                if (downTime == 0)
    //                    size = originalSize;
    //                else
    //                    size = Vector3.Lerp(originalSize, pumpSize, leftTime / downTime);
    //                cachedTransform.localScale = size;
    //                if(leftTime == 0 && onPumpEnd != null)
    //                    onPumpEnd.Invoke();
    //            }
    //            break;
    //    }
    //}

    [Serializable]
    public class PumpEvent : UnityEvent { }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayedEventScript : MonoBehaviour
{
    public float time;
    public GameObject pauseObj;
    public DelayedEvent onEnd;
    public bool isUnscaledTime;
    // Start is called before the first frame update
    private void OnEnable()
    {
        StartCoroutine(DelayCorou());
    }

    IEnumerator DelayCorou()
    {
        float t = time;
        do
        {
            yield return null;
            if(!pauseObj || !pauseObj.activeSelf)
                t -= isUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        } while (t > 0);
        if(onEnd != null)
            onEnd.Invoke();
    }


    [System.Serializable]
    public class DelayedEvent : UnityEvent
    {
    }
}

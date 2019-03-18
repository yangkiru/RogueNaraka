using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayedEventScript : MonoBehaviour
{
    public float time;
    public DelayedEvent onEnd;
    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(DelayCorou());
    }

    IEnumerator DelayCorou()
    {
        float t = time;
        do
        {
            yield return null;
            
            t -= Time.deltaTime;
        } while (t > 0);
        if(onEnd != null)
            onEnd.Invoke();
    }


    [System.Serializable]
    public class DelayedEvent : UnityEvent
    {
    }
}

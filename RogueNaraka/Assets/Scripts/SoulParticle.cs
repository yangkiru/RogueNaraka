using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGSpline.Curve;
using BansheeGz.BGSpline.Components;

public class SoulParticle : MonoBehaviour
{
    public Transform[] points;
    public BGCcCursorChangeLinear bgc;

    void OnEnable()
    {
        StartCoroutine(Corou());
    }

    IEnumerator Corou()
    {
        float t = Random.Range(0f, 1f);
        do
        {
            yield return null;
            t -= Time.deltaTime;
        } while (t > 0);
        float a = 0;
        do
        {
            yield return null;
            bgc.Speed += 0.1f + a;
            a += 0.005f;
        } while (bgc.Speed < 1);
        
        do
        {
            yield return null;
            bgc.Speed += 0.1f + a;
            
        } while (bgc.Speed < 4);
    }

    public void OnReached()
    {

    }
}

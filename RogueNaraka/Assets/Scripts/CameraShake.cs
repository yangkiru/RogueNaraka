using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    static public CameraShake instance;
    public new Camera camera;
    public float time;
    public float power;
    public float speed;
    private Vector3 origin = Vector3.zero;

    private void Awake()
    {
        instance = this;
    }
    public void Shake(float time, float power, float gap, bool isUnscale = false)
    {
        StartCoroutine(RandomMove(time, power, gap, isUnscale));
    }

    public void Shake(ShakeData data, bool isUnscale = false)
    {
        Shake(data.time, data.power, data.gap, isUnscale);
    }
    private IEnumerator RandomMove(float time, float power, float gap, bool isUnscale = false)
    {
        float t1 = 0, t2 = 0;
        if (gap <= 0)
            gap = 0.001f;
        if(origin == Vector3.zero)
            origin = camera.transform.position;
        while (t1 <= time)
        {
            Vector3 random = new Vector3(Random.Range(-power, power), Random.Range(-power, power), origin.z);
            camera.transform.position = random;
            
            while (t2 <= gap)
            {
                yield return null;
                t1 += isUnscale ? Time.unscaledDeltaTime : Time.deltaTime;
                t2 += isUnscale ? Time.unscaledDeltaTime : Time.deltaTime;
            }
            t2 = 0;
            
            camera.transform.position = origin;
        }
    }
}

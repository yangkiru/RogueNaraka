using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    public Camera camera;
    public float time;
    public float power;
    public float speed;
    private Vector3 origin;
	public void Shake(float time, float power, float gap)
    {
        StartCoroutine(RandomMove(time, power, gap));
    }

    public void Shake(ShakeData data)
    {
        Shake(data.time, data.power, data.gap);
    }
    private IEnumerator RandomMove(float time, float power, float gap)
    {
        float t1 = 0, t2 = 0;
        if (gap <= 0)
            gap = 0.001f;
        origin = camera.transform.position;
        while (t1 <= time)
        {
            Vector3 random = new Vector3(Random.Range(-1f, 1f) * power, Random.Range(-1f, 1f) * power, origin.z);
            camera.transform.position = random;
            
            while (t2 <= gap)
            {
                yield return null;
                t1 += Time.deltaTime;
                t2 += Time.deltaTime;
            }
            t2 = 0;
            
            camera.transform.position = origin;
        }
    }
}

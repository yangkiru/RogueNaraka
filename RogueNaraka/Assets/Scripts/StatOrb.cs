using BansheeGz.BGSpline.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatOrb : MonoBehaviour
{
    [SerializeField]
    STAT type;

    [SerializeField]
    ParticleSystem particle;
    [SerializeField]
    BGCcCursor cursor;
    Transform from;
    Transform to;
    [SerializeField]
    Transform[] point;
    Vector2 origin;

    Stat stat;

    public event System.Action<STAT, Stat> onEnd;

    public void Play(STAT type, Stat stat = null, System.Action<STAT, Stat> onEnd = null)
    {
        gameObject.SetActive(true);
        this.type = type;
        to = StatOrbManager.instance.stat[(int)type];
        from = StatOrbManager.instance.from;
        transform.position = from.position;
        origin = point[1].position;
        float power = StatOrbManager.instance.rndPower;
        point[1].position = new Vector2(point[1].position.x + Random.Range(-power, power)
            , point[1].position.y + Random.Range(-power, power));
        point[2].position = to.transform.position;
        cursor.DistanceRatio = 0;
        Color color = StatOrbManager.instance.statColor[(int)type];
        var main = particle.main;
        main.startColor = color;
        if (onEnd != null)
            this.onEnd = onEnd;
        else
            this.onEnd = null;

        this.stat = stat;
    }

    private void Update()
    {
        if (cursor.DistanceRatio >= 1)
            OnEnd();
    }

    void OnEnd()
    {
        point[1].transform.position = origin;
        //Debug.Log(type);
        ParticleSystem end = StatOrbManager.instance.endPool.DequeueObjectPool().GetComponent<ParticleSystem>();
        end.transform.position = point[2].position;
        end.gameObject.SetActive(true);
        var main = end.main;
        main.startColor = particle.main.startColor;
        end.Play();
        if (onEnd != null)
            onEnd.Invoke(type, stat);
        StatOrbManager.instance.orbPool.EnqueueObjectPool(gameObject);
    }
}

using BansheeGz.BGSpline.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatOrb : MonoBehaviour
{
    [SerializeField]
    STAT stat;

    [SerializeField]
    ParticleSystem particle;
    [SerializeField]
    BGCcCursor cursor;
    Transform from;
    Transform to;
    [SerializeField]
    Transform[] point;
    Vector2 origin;

    public void Play(STAT stat)
    {
        gameObject.SetActive(true);
        this.stat = stat;
        to = StatOrbManager.instance.stat[(int)stat];
        from = StatOrbManager.instance.from;
        transform.position = from.position;
        origin = point[1].position;
        float power = StatOrbManager.instance.rndPower;
        point[1].position = new Vector2(point[1].position.x + Random.Range(-power, power)
            , point[1].position.y + Random.Range(-power, power));
        point[2].position = to.transform.position;
        cursor.DistanceRatio = 0;
        Color color = StatOrbManager.instance.statColor[(int)stat];
        var main = particle.main;
        main.startColor = color;
    }

    private void Update()
    {
        if (cursor.DistanceRatio >= 1)
            OnEnd();
    }

    void OnEnd()
    {
        point[1].transform.position = origin;
        Debug.Log(stat);
        //GameManager.instance.player.stat.AddOrigin(stat, 1);
        ParticleSystem end = StatOrbManager.instance.endPool.DequeueObjectPool().GetComponent<ParticleSystem>();
        end.transform.position = point[2].position;
        end.gameObject.SetActive(true);
        var main = end.main;
        main.startColor = particle.main.startColor;
        end.Play();
        StatOrbManager.instance.orbPool.EnqueueObjectPool(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGSpline.Curve;
using BansheeGz.BGSpline.Components;

public class SoulParticle : MonoBehaviour
{
    public Transform[] points;
    public BGCcCursorChangeLinear bgc;
    public Rigidbody2D rigid;
    public SpriteRenderer spr;

    public static int soulCount = 0;

    int soul;

    public void Init(Vector3 position, int soul)
    {
        this.soul = soul;
        points[0].position = position;
        gameObject.SetActive(true);
        soulCount++;
    }

    void OnEnable()
    {
        Vector3 rndVec = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        float rndPower = Random.Range(1f, 2f);
        rigid.AddRelativeForce(rndVec * rndPower, ForceMode2D.Impulse);
        StartCoroutine(Corou());
    }

    IEnumerator Corou()
    {
        yield return null;
        spr.enabled = true;
        float t = Random.Range(1f, 2f);
        do
        {
            yield return null;
            t -= Time.deltaTime;
        } while (t > 0);

        float x = Mathf.Lerp(points[0].position.x, points[2].position.x, Random.Range(0f, 1f));
        float y = Mathf.Lerp(points[0].position.y, points[2].position.y, Random.Range(0f, 1f));
        points[1].position = new Vector3(x, y);

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
            
        } while (bgc.Speed < 5);
    }

    public void OnReached(int point)
    {
        if (point != 2)
            return;
        if (--soulCount <= 0)
            LevelUpManager.instance.RequestEndStageCorou();
        bgc.Speed = 0;
        MoneyManager.instance.AddSoul(soul, false);
        BoardManager.instance.soulPool.EnqueueObjectPool(gameObject);
    }
}

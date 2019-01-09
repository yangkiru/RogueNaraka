using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGSpline.Components;
using BansheeGz.BGSpline.Curve;
public class StatOrb : MonoBehaviour
{
    public BGCcTrs trs;
    public BGCcCursor cursor;
    public GameObject startPoint;
    public GameObject endPoint;
    public GameObject orb;
    public Rigidbody2D rigid;
    public float speed;

    bool isShaked;

    private void OnEnable()
    {
        trs.MoveObject = false;
        orb.transform.localPosition = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
    }
    private void OnDisable()
    {
        rigid.velocity = Vector2.zero;
        speed = 0;
        isShaked = false;
    }

    private void Update()
    {
        speed = Mathf.Abs(rigid.velocity.x) + Mathf.Abs(rigid.velocity.y);
        if (speed > 10)
        {
            rigid.velocity = rigid.velocity * 0.9f;
        }

        if (!isShaked && trs.MoveObject)
        {
            int from, to;
            cursor.GetAdjacentPointIndexes(out from, out to);
            if (from == 1)
            {
                CameraShake.instance.Shake(0.1f, 0.1f, 0.001f);
                isShaked = true;
            }
        }
    }
}

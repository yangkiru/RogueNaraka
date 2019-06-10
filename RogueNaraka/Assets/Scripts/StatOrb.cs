using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BansheeGz.BGSpline.Components;
using BansheeGz.BGSpline.Curve;
public class StatOrb : MonoBehaviour
{
    public BGCcTrs trs;
    public BGCcCursor cursor;
    public GameObject startPoint;
    public GameObject endPoint;
    public Image StatOrbImage;
    public Sprite[] StatOrbSpriteArr;
    public Rigidbody2D rigid;
    public float speed;

    bool isShaked;

    private int value;
    public int Value { get { return value; } }

    private void OnEnable()
    {
        trs.MoveObject = false;
        StatOrbImage.transform.localPosition = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
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

    public void Init(int _value) {
        this.value = _value;
        int spriteIdx = this.value / 2;
        if(spriteIdx > this.StatOrbSpriteArr.Length) {
            throw new System.ArgumentException("value is over max value of statOrb");
        }
        this.StatOrbImage.sprite = this.StatOrbSpriteArr[spriteIdx];
    }
}

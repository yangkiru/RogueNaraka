using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour {

    public EffectData data
    { get { return _data; } }
    [SerializeField][ReadOnly]
    private EffectData _data;

    public new SpriteRenderer renderer;
    public Unit owner;

    public bool isActive
    { get { return _isActive; } }
    private bool _isActive;

    public bool isBuff
    { get { return false; } }//버프를 추가 ex)switch(data.type){case EFFECT.someBuff:case EFFECT.someBuff2:return true;default:return false;};

    private IEnumerator coroutine;

    public void SetData(EffectData dt)
    {
        _data = dt;
        EffectSpriteData sprData = GameDatabase.instance.effects[(int)dt.type];
        name = sprData.name;
        renderer.sprite = sprData.spr;
    }

    public void SetOwner(Unit unit)
    {
        owner = unit;
    }

    public void Active(bool value)
    {
        if(value)
        {
            _isActive = true;
            if (coroutine == null && !_data.isInfinity)
            {
                coroutine = TimeCoroutine();
                StartCoroutine(coroutine);
            }
        }
        else
        {
            _isActive = false;
            if (coroutine != null)
                StopCoroutine(coroutine);
        }
    }

    private IEnumerator TimeCoroutine()
    {
        while(_data.time > 0 && !owner.isDeath)
        {
            yield return null;
            _data.time -= Time.deltaTime;
        }
        DestroySelf();
    }

    public void DestroySelf()
    {
        owner.effects.Remove(this);
        owner = null;
        BoardManager.instance.effectPool.EnqueueObjectPool(gameObject, false);
    }
}

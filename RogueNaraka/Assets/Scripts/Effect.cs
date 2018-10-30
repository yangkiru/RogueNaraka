using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour {

    public EffectData data
    { get { return _data; } }
    [SerializeField][ReadOnly]
    private EffectData _data;

    public bool isActive
    { get { return _isActive; } }
    private bool _isActive;

    public bool isBuff
    { get { return false; } }//버프를 추가 ex)switch(data.type){case EFFECT.someBuff:case EFFECT.someBuff2:return true;default:return false;};

    private IEnumerator coroutine;

    public void SetData(EffectData dt)
    {
        _data = dt;
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
        while(_data.time > 0)
        {
            yield return null;
            _data.time -= Time.deltaTime;
        }
        DestroySelf();
    }

    public void DestroySelf()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        Destroy(this);
    }
}

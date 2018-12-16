using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;

public class Effect : MonoBehaviour {

    public EffectData data
    { get { return _data; } }
    [SerializeField][ReadOnly]
    private EffectData _data;

    public new SpriteRenderer renderer;
    public Unit owner;

    public void Init(Unit owner, EffectData data)
    {
        _data = data;
        EffectSpriteData sprData = GameDatabase.instance.effects[(int)data.type];
        name = sprData.name;
        renderer.sprite = sprData.spr;
        this.owner = owner;
        transform.SetParent(owner.effectable.holder);
    }

    void Update()
    {
        if(_data.time > 0 && !owner.deathable.isDeath)
            _data.time -= Time.deltaTime;
        else
            Destroy();
    }

    public void Destroy()
    {
        owner.effectable.effects.Remove(this);
        owner = null;
        _data = null;
        BoardManager.instance.effectPool.EnqueueObjectPool(gameObject);
    }
}

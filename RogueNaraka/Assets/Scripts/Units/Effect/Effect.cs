using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.EffectScripts
{
    public abstract class Effect : MonoBehaviour
    {
        public EffectData data
        { get { return _data; } }
        [SerializeField]
        private EffectData _data;

        //public new SpriteRenderer renderer;
        public Unit owner;
        List<Effect> list;

        public void Init(Unit owner, EffectData data, List<Effect> list)
        {
            _data = data;
            EffectSpriteData sprData = GameDatabase.instance.effects[(int)data.type];
            name = sprData.name;
            GetComponent<SpriteRenderer>().sprite = sprData.spr;
            this.owner = owner;
            transform.SetParent(owner.effectable.holder);
            gameObject.SetActive(true);
            this.list = list;
            list.Add(this);
            OnInit();
        }

        void Update()
        {
            if (_data.time > 0 && !owner.deathable.isDeath)
                _data.time -= Time.deltaTime;
            else
                Destroy();
        }

        public void Destroy()
        {
            OnDestroyEffect();
            owner.effectable.effects.Remove(this);
            list.Remove(this);
            BoardManager.instance.effectPool.EnqueueObjectPool(gameObject);
            Destroy(this);
        }

        protected abstract void OnInit();
        protected abstract void OnDestroyEffect();
        public abstract void Combine(EffectData dt);
    }
}
//public void CombineEffects(Effect result, EffectData data)
//{
//    switch (data.type)
//    {
//        case EFFECT.STUN:
//            result.data.time += data.time;
//            break;
//        case EFFECT.SLOW:
//            break;
//        case EFFECT.FIRE:
//            break;
//        case EFFECT.ICE:
//            result.data.time += data.time;
//            break;
//        case EFFECT.KNOCKBACK:
//            result.data.value += data.value;
//            break;
//        case EFFECT.POISON:
//            break;
//        case EFFECT.HEAL:
//            break;
//        case EFFECT.LIFESTEAL:
//            break;
//    }
//}
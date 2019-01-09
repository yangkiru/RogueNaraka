using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;
using RogueNaraka.BulletScripts;

namespace RogueNaraka.EffectScripts
{
    public abstract class Effect : MonoBehaviour
    {
        public EffectData data
        { get { return _data; } }
        [SerializeField]
        private EffectData _data;

        //public new SpriteRenderer renderer;
        public Unit target { get { return _target; } }
        [SerializeField]
        Unit _target;//A to B, B
        public Unit owner { get { return _owner; } }
        [SerializeField]
        Unit _owner;//A to B, A
        public Bullet bullet { get { return _bullet; } }
        [SerializeField]
        Bullet _bullet;
        List<Effect> list;

        public void Init(EffectData data, List<Effect> list, Unit target, Bullet bullet = null, Unit owner = null)
        {
            _data = data;
            EffectSpriteData sprData = GameDatabase.instance.effects[(int)data.type];
            name = sprData.name;
            GetComponent<SpriteRenderer>().sprite = sprData.spr;
            if (sprData.spr == null)
                transform.SetAsLastSibling();
            _owner = owner;
            _target = target;
            _bullet = bullet;
            transform.SetParent(_target.effectable.holder);
            gameObject.SetActive(true);
            this.list = list;
            list.Add(this);
            OnInit();
        }

        void Update()
        {
            if (_data.time > 0 && !target.deathable.isDeath)
                _data.time -= Time.deltaTime;
            else
                Destroy();
        }

        public void Destroy()
        {
            OnDestroyEffect();
            target.effectable.effects.Remove(this);
            list.Remove(this);
            BoardManager.instance.effectPool.EnqueueObjectPool(gameObject);
            Destroy(this);
        }



        protected abstract void OnInit();
        protected abstract void OnDestroyEffect();
        public abstract void Combine(EffectData dt);
        public abstract bool Equal(EffectData dt);
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
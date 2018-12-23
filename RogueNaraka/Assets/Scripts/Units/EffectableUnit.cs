using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.EffectScripts;

namespace RogueNaraka.UnitScripts
{
    public class EffectableUnit : MonoBehaviour
    {
        public Transform holder;
        [SerializeField]
        Unit unit;
        public List<Effect> effects { get { return _effects; } }
        [SerializeField]
        List<Effect> _effects = new List<Effect>();

        public List<Effect> stun { get { return _stun; } }
        List<Effect> _stun = new List<Effect>();
        public List<Effect> slow { get { return _slow; } }
        List<Effect> _slow = new List<Effect>();
        public List<Effect> ice { get { return _ice; } }
        [SerializeField]
        List<Effect> _ice = new List<Effect>();
        public List<Effect> fire { get { return _fire; } }
        List<Effect> _fire = new List<Effect>();
        public List<Effect> knockback { get { return _knockback; } }
        List<Effect> _knockback = new List<Effect>();
        public List<Effect> poison { get { return _poison; } }
        List<Effect> _poison = new List<Effect>();
        public List<Effect> heal { get { return _heal; } }
        List<Effect> _heal = new List<Effect>();
        public List<Effect> lifeSteal { get { return _lifeSteal; } }
        List<Effect> _lifeSteal = new List<Effect>();

        void Reset()
        {
            unit = GetComponent<Unit>();
        }

        public void Init()
        {
            for(int i = 0; i < _effects.Count; i++)
            {
                _effects[i].Destroy();
            }
            
            _stun.Clear();
            _slow.Clear();
            _ice.Clear();
            _fire.Clear();
            _knockback.Clear();
            _poison.Clear();
            _heal.Clear();
            _lifeSteal.Clear();
        }

        public Effect GetSameEffect(EffectData data)
        {
            
            switch (data.type)
            {
                case EFFECT.STUN:
                    if (_stun.Count > 0)
                        return _stun[0];
                    else
                        return null;
                case EFFECT.SLOW:
                    return null;
                case EFFECT.FIRE:
                    return null;
                case EFFECT.ICE:
                    for (int i = 0; i < _ice.Count; i++)
                    {
                        if (_ice[i].data.value == data.value)
                            return _ice[i];
                    }
                    return null;
                case EFFECT.KNOCKBACK:
                    for (int i = 0; i < _knockback.Count; i++)
                    {
                        if (_knockback[i].data.time == data.time)
                            return _knockback[i];
                    }
                    return null;
                case EFFECT.POISON:
                    return null;
                case EFFECT.HEAL:
                    return null;
                case EFFECT.LIFESTEAL:
                    return null;
                default:
                    return null;
            }
        }

        public void AddEffect(EffectData data)
        {
            Effect effect = GetSameEffect(data);
            GameObject obj = effect == null ? BoardManager.instance.effectPool.DequeueObjectPool() : null;
            List<Effect> list = null;
            if (!effect)
            {
                switch (data.type)
                {
                    case EFFECT.STUN:
                        //effect = obj.AddComponent<Stun>();
                        list = _stun;
                        break;
                    case EFFECT.SLOW:
                        //effect = obj.AddComponent<Slow>();
                        list = _slow;
                        break;
                    case EFFECT.FIRE:
                        //effect = obj.AddComponent<Fire>();
                        list = _fire;
                        break;
                    case EFFECT.ICE:
                        effect = obj.AddComponent<Ice>();
                        list = _ice;
                        break;
                    case EFFECT.KNOCKBACK:
                        //effect = obj.AddComponent<Knockback>();
                        list = _knockback;
                        break;
                    case EFFECT.POISON:
                        //effect = obj.AddComponent<Poison>();
                        list = _poison;
                        break;
                    case EFFECT.HEAL:
                        //effect = obj.AddComponent<Heal>();
                        list = _heal;
                        break;
                    case EFFECT.LIFESTEAL:
                        //effect = obj.AddComponent<LifeSteal>();
                        list = _lifeSteal;
                        break;
                }

                effect.Init(unit, (EffectData)data.Clone(), list);
            }
            else
            {
                effect.Combine(data);
            }
        }
    }
}

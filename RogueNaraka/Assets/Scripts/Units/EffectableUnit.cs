using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts
{
    public class EffectableUnit : MonoBehaviour
    {
        public Transform holder;
        Unit unit;
        public List<Effect> effects { get { return _effects; } }
        [SerializeField]
        List<Effect> _effects = new List<Effect>();

        public List<Effect> stun { get { return _stun; } }
        List<Effect> _stun = new List<Effect>();
        public List<Effect> slow { get { return _slow; } }
        List<Effect> _slow = new List<Effect>();
        public List<Effect> ice { get { return _ice; } }
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

        public void AddEffect(EffectData data)
        {
            Effect effect = BoardManager.instance.effectPool.DequeueObjectPool().GetComponent<Effect>();
            switch(data.type)
            {
                case EFFECT.STUN:
                    _stun.Add(effect);
                    break;
                case EFFECT.SLOW:
                    _slow.Add(effect);
                    break;
                case EFFECT.FIRE:
                    _fire.Add(effect);
                    break;
                case EFFECT.ICE:
                    _ice.Add(effect);
                    break;
                case EFFECT.KNOCKBACK:
                    _knockback.Add(effect);
                    break;
                case EFFECT.POISON:
                    _poison.Add(effect);
                    break;
                case EFFECT.HEAL:
                    _heal.Add(effect);
                    break;
                case EFFECT.LIFESTEAL:
                    _lifeSteal.Add(effect);
                    break;
            }
            effect.Init(unit, (EffectData)data.Clone());
        }
    }
}

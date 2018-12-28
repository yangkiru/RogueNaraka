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

        Dictionary<EFFECT, List<Effect>> dictionary = new Dictionary<EFFECT, List<Effect>>();

        void Reset()
        {
            unit = GetComponent<Unit>();
        }

        private void Awake()
        {
            for(int i = 0; i < (int)EFFECT.LifeSteal; i++)
            {
                dictionary.Add((EFFECT)i, new List<Effect>());
            }
        }

        public void Init()
        {
            for(int i = 0; i < _effects.Count; i++)
            {
                _effects[i].Destroy();
            }

            _effects.Clear();
            dictionary.Clear();
        }

        public Effect GetSameEffect(EffectData data)
        {
            List<Effect> list = dictionary[data.type];
            switch (data.type)
            {
                case EFFECT.Stun:
                    if (list.Count > 0)
                        return list[0];
                    else
                        return null;
                case EFFECT.Slow:
                    return null;
                case EFFECT.Fire:
                    return null;
                case EFFECT.Ice:
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].data.value == data.value)
                            return list[i];
                    }
                    return null;
                case EFFECT.Knockback:
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].data.time == data.time)
                            return list[i];
                    }
                    return null;
                case EFFECT.Poison:
                    return null;
                case EFFECT.Heal:
                    return null;
                case EFFECT.LifeSteal:
                    return null;
                default:
                    return null;
            }
        }

        public void AddEffect(EffectData data)
        {
            Effect effect = GetSameEffect(data);
            GameObject obj = effect == null ? BoardManager.instance.effectPool.DequeueObjectPool() : null;

            if (!effect)
            {
                System.Type type = System.Type.GetType(string.Format("RogueNaraka.EffectScripts.{0}", data.type));
                effect = obj.AddComponent(type) as Effect;

                List<Effect> list = dictionary[data.type];

                effect.Init(unit, (EffectData)data.Clone(), list);
            }
            else
            {
                effect.Combine(data);
            }
        }
    }
}

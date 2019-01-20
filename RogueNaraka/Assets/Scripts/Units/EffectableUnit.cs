using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.EffectScripts;
using RogueNaraka.BulletScripts;

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

        public float effectDelay { get { return _effectDelay; } }
        float _effectDelay = 0.1f;

        void Reset()
        {
            unit = GetComponent<Unit>();
        }

        private void Start()
        {
            for(int i = 0; i < GameDatabase.instance.effects.Length; i++)
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
            for(int i = 0; i < list.Count; i++)
            {
                if (list[i].Equal(data))
                    return list[i];
            }
            return null;
        }

        public void AddEffect(EffectData data, Bullet bullet = null, Unit owner = null)
        {
            Effect effect = GetSameEffect(data);
            GameObject obj = effect == null ? BoardManager.instance.effectPool.DequeueObjectPool() : null;

            if (!effect)
            {
                System.Type type = System.Type.GetType(string.Format("RogueNaraka.EffectScripts.{0}", data.type));
                effect = obj.AddComponent(type) as Effect;

                List<Effect> list = dictionary[data.type];

                effect.Init((EffectData)data.Clone(), list, unit, bullet, owner);
            }
            else
            {
                effect.Combine(data);
            }
        }

        public void AddEffect(EFFECT type, float value, float time, Bullet bullet = null, Unit owner = null)
        {
            EffectData effect = new EffectData(type, value, time);
            AddEffect(effect, bullet, owner);
        }
    }
}

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

        public float effectDelay = 1;

        [SerializeField]
        protected float[] resistances = new float[(int)EFFECT.Accel+1];

        void Reset()
        {
            unit = GetComponent<Unit>();
        }

        public void Init()
        {
            _effects.Clear();
            dictionary.Clear();
            for (int i = 0; i < GameDatabase.instance.effects.Length; i++)
            {
                dictionary.Add((EFFECT)i, new List<Effect>());
            }
            InitResistance();
        }

        private void OnDisable()
        {
            if (!BoardManager.instance.effectPool)
                return;
            for(int i = 0; i < _effects.Count; i++)
            {
                _effects[i].Destroy();
            }
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

        public Effect AddEffect(EffectData data, Bullet bullet = null, Unit owner = null)
        {
            Effect effect = GetSameEffect(data);
            GameObject obj = effect == null ? BoardManager.instance.effectPool.DequeueObjectPool() : null;

            if (!effect)
            {
                System.Type type = System.Type.GetType(string.Format("RogueNaraka.EffectScripts.{0}", data.type));
                effect = obj.AddComponent(type) as Effect;

                List<Effect> list = dictionary[data.type];

                effects.Add(effect);
                effect.Init((EffectData)data.Clone(), list, unit, bullet, owner);
            }
            else
            {
                effect.Combine(data);
            }
            return effect;
        }

        public Effect AddEffect(EFFECT type, float value, float time, Bullet bullet = null, Unit owner = null)
        {
            EffectData effect = new EffectData(type, value, time);
            return AddEffect(effect, bullet, owner);
        }

        public void InitResistance()
        {
            for(int i = 0; i < resistances.Length; i++)
            {
                resistances[i] = 0;
            }
            AddResistance(unit.data.resistances);
        }

        public void AddResistance(params EffectResistance[] resistances)
        {
            for (int i = 0; i < resistances.Length; i++)
            {
                this.resistances[(int)resistances[i].type] += resistances[i].value;
            }
        }

        public float GetResistance(EFFECT type)
        {
            return resistances[(int)type];
        }
    }
}

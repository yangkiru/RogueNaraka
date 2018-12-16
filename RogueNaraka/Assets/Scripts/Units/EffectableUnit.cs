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
        }

        public void AddEffect(EffectData data)
        {
            Effect effect = BoardManager.instance.effectPool.DequeueObjectPool().GetComponent<Effect>();
            effect.Init(unit, (EffectData)data.Clone());
        }
    }
}

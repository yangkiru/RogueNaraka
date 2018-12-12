using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts
{
    public class EffectableUnit : MonoBehaviour
    {

        Unit unit;
        public List<Effect> effects { get { return _effects; } }
        List<Effect> _effects = new List<Effect>();

        void Awake()
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

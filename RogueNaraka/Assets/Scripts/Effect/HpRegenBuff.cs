using UnityEngine;
using System.Collections;

namespace RogueNaraka.EffectScripts{
    public class HpRegenBuff : Effect
    {
        public override void Combine(EffectData dt)
        {
            data.time += dt.time;
        }

        public override bool Equal(EffectData dt)
        {
            return dt.value == data.value;
        }

        protected override void OnDestroyEffect()
        {
            target.data.hpRegen -= data.value;
        }

        protected override void OnInit()
        {
            target.data.hpRegen += data.value;
        }
    }
}
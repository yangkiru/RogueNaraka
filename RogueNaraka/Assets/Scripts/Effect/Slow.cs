using UnityEngine;
using System.Collections;
using RogueNaraka.EffectScripts;

namespace RogueNaraka.EffectScripts{
    public class Slow : Effect
    {
        float cached;
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
            target.stat.AddTemp(STAT.SPD, -cached);
        }

        protected override void OnInit()
        {
            cached = -target.stat.spd * data.value;
            target.stat.AddTemp(STAT.SPD, cached);
        }
    }
}

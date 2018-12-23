using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.EffectScripts
{
    public class Ice : Effect
    {
        public override void Combine(EffectData dt)
        {
            data.time += dt.time;
        }

        protected override void OnInit()
        {
            owner.stat.tmpSpd -= data.value;
        }

        protected override void OnDestroyEffect()
        {
            owner.stat.tmpSpd += data.value;
        }
    }
}
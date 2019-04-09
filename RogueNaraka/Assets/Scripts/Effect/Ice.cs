using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.EffectScripts
{
    public class Ice : Effect
    {
        IEnumerator decelerateCorou;
        IEnumerator DecelerateCorou()
        {
            yield return null;
            
            while (target.moveable.CurSpeed > target.stat.spdTemp)
            {
                target.moveable.ForceToDecelerate(0.1f);
                yield return null;
            }
            decelerateCorou = null;
        }
        public override void Combine(EffectData dt)
        {
            this.data.time = dt.time;
        }

        protected override void OnInit()
        {
            target.stat.spdTemp -= data.value;
            decelerateCorou = DecelerateCorou();
            StartCoroutine(decelerateCorou);
        }

        protected override void OnDestroyEffect()
        {
            target.stat.spdTemp += data.value;
            if(decelerateCorou != null)
            {
                StopCoroutine(decelerateCorou);
                decelerateCorou = null;
            }
        }

        public override bool Equal(EffectData dt)
        {
            return data.value == dt.value;
        }
    }
}
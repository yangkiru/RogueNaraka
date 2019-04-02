using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.EffectScripts
{
    public class Stun : Effect
    {
        bool isStunParam;

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
            data.time += dt.time;
        }

        protected override void OnInit()
        {
            target.isStun = true;
            try
            {
                target.animator.SetBool("isStun", true);
                isStunParam = true;
            }
            catch
            {
                isStunParam = false;
            }
        }

        

        protected override void OnDestroyEffect()
        {
            target.isStun = false;
            if (isStunParam)
                target.animator.SetBool("isStun", false);
        }

        public override bool Equal(EffectData dt)
        {
            return data.value == dt.value;
        }
    }
}
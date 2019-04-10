using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.EffectScripts
{
    public class Stun : Effect
    {
        bool isStunParam;

        public override void Combine(EffectData dt)
        {
            data.time = Mathf.Max(data.time, dt.time);
        }

        protected override void OnInit()
        {
            target.isStun = true;

            AnimatorControllerParameter[] parameters = target.animator.parameters;

            for(int i = 0; i < parameters.Length; i++)
            {
                if(parameters[i].name.CompareTo("isStun") == 0)
                {
                    target.animator.SetBool("isStun", true);
                    isStunParam = true;
                    break;
                }
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
            return true;
        }
    }
}
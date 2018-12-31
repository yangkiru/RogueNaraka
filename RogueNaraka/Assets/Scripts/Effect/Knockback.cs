using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.EffectScripts
{
    //value means angle of knockback
    //time means power of knockback
    public class Knockback : Effect
    {
        public override void Combine(EffectData dt)
        {
            data.time += dt.time;
        }

        protected override void OnInit()
        {
            Vector2 vec;
            if (bullet)
            {
                vec = (bullet.transform.position - target.transform.position).normalized;
                data.value = MathHelpers.Vector2ToDegree(vec);
            }
            else if (owner)
            {
                vec = (owner.transform.position - target.transform.position).normalized;
                data.value = MathHelpers.Vector2ToDegree(vec);
            }
            else
            {
                vec = MathHelpers.DegreeToVector2(data.value).normalized;
            }
            
            owner.rigid.AddForce(vec * data.time);
            Destroy();
        }

        protected override void OnDestroyEffect()
        {
            
        }

        public override bool Equal(EffectData dt)
        {
            return data.value == dt.value;
        }
    }
}
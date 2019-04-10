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
                vec = (target.transform.position - bullet.transform.position).normalized;
                data.value = MathHelpers.Vector2ToDegree(vec);
            }
            else if (owner)
            {
                vec = (target.transform.position - owner.transform.position).normalized;
                data.value = MathHelpers.Vector2ToDegree(vec);
            }
            else
            {
                vec = MathHelpers.DegreeToVector2(data.value).normalized;
            }

            target.rigid.AddForce(vec * data.time * (1 - target.effectable.GetResistance(EFFECT.Knockback)));
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
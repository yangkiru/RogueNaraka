using UnityEngine;
using System.Collections;

namespace RogueNaraka.EffectScripts
{
    public class Fire : Effect
    {
        Color color;
        IEnumerator damageCorou;

        public override void Combine(EffectData dt)
        {
            data.time = Mathf.Max(data.time, dt.time);
        }

        public override bool Equal(EffectData dt)
        {
            return data.value == dt.value;
        }

        protected override void OnDestroyEffect()
        {
            if (damageCorou != null)
            {
                StopCoroutine(damageCorou);
                damageCorou = null;
            }
            target.renderer.color = color;
        }

        protected override void OnInit()
        {
            color = target.renderer.color;
            if (damageCorou == null)
            {
                damageCorou = Damage();
                StartCoroutine(damageCorou);
            }
            target.renderer.color = new Color(1, 0.368f, 0);
        }

        IEnumerator Damage()
        {
            while (true)
            {
                float t = 0.5f * target.effectable.effectDelay;
                
                do
                {
                    yield return null;
                    t -= Time.deltaTime;
                } while (t > 0);
                target.damageable.Damage(data.value);
            }
        }
    }
}
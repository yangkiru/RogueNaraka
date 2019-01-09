﻿using UnityEngine;
using System.Collections;

namespace RogueNaraka.EffectScripts
{
    public class Fire : Effect
    {
        Color color;

        public override void Combine(EffectData dt)
        {
            data.time += dt.value;
        }

        public override bool Equal(EffectData dt)
        {
            return data.value == dt.value;
        }

        protected override void OnDestroyEffect()
        {
            StopCoroutine("Damage");
            target.renderer.color = color;
        }

        protected override void OnInit()
        {
            color = target.renderer.color;
            StartCoroutine("Damage");
            target.renderer.color = new Color(1, 0.368f, 0);
        }

        IEnumerator Damage()
        {
            while (true)
            {
                float t = target.effectable.effectDelay;
                
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
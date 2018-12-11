using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.BulletScripts
{
    public class DamageableBullet : MonoBehaviour
    {
        [SerializeField]
        float damage;

        public void Damage(Unit unit)
        {
            unit.damageable.Damage(damage);
        }
    }
}

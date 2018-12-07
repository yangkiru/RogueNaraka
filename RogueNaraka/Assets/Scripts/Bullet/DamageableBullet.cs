using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.Bullet
{
    public class DamageableBullet : MonoBehaviour
    {
        [SerializeField]
        float damage;

        public void Damage(Unit unit)
        {
            unit.GetDamage(damage);
        }
    }
}

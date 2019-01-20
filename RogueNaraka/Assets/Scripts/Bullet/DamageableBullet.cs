using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.BulletScripts
{
    public class DamageableBullet : MonoBehaviour
    {
        [SerializeField]
        Bullet bullet;

        void Reset()
        {
            bullet = GetComponent<Bullet>();
        }

        public float Damage(Unit unit, STAT related = STAT.DMG)
        {
            float ownerDmg = bullet.ownerable.unit ? bullet.ownerable.unit.stat.GetCurrent(related) : 1;
            float damage = bullet.data.dmg * ownerDmg;
            //Debug.Log(string.Format("{0}'s {1} damaged {2} to {3}", bullet.ownerable.unit.name, name, damage, unit.name));
            
            unit.damageable.Damage(damage);
            return damage;
        }
    }
}

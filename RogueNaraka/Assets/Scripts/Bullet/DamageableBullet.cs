﻿using System.Collections;
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

        public void Damage(Unit unit)
        {
            float ownerDmg = bullet.ownerable.owner ? bullet.ownerable.owner.data.stat.dmg : 1;
            float damage = bullet.data.dmg * ownerDmg;
            Debug.Log(string.Format("{0}'s {1} damaged {2} to {3}", bullet.ownerable.owner.name, name, damage, unit.name));
            unit.damageable.Damage(damage);
        }
    }
}

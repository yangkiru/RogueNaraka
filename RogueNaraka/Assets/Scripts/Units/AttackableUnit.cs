using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.BulletScripts;
using RogueNaraka.UnitScripts.Targetable;

namespace RogueNaraka.UnitScripts
{
    public class AttackableUnit : MonoBehaviour
    {

        WeaponData weapon;
        Unit owner;

        void Awake()
        {
            owner = GetComponent<Unit>();
        }

        public void Init(UnitData data)
        {
            weapon = GameDatabase.instance.weapons[data.weaponId];
        }

        public void Attack()
        {
            Bullet bullet = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
            bullet.Spawn(owner, GameDatabase.instance.bullets[weapon.startBulletId], transform.position + weapon.offset);
            bullet.shootable.Shoot(owner.targetable.direction, bullet.data.speed, bullet.data.accel);
        }
    }
}
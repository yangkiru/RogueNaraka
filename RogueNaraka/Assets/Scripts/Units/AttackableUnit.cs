using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.BulletScripts;
using RogueNaraka.UnitScripts.Targetable;

namespace RogueNaraka.UnitScripts
{
    public class AttackableUnit : MonoBehaviour
    {

        public WeaponData weapon { get { return _weapon; } }
        WeaponData _weapon;
        Unit owner;

        void Awake()
        {
            owner = GetComponent<Unit>();
        }

        public void Init(UnitData data)
        {
            _weapon = GameDatabase.instance.weapons[data.weapon];
        }

        public void Init(WeaponData data)
        {
            _weapon = data;
        }

        public void Attack()
        {
            Bullet bullet = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
            bullet.Spawn(owner, GameDatabase.instance.bullets[_weapon.startBulletId], transform.position + _weapon.offset);
            bullet.shootable.Shoot(owner.targetable.direction, bullet.data.speed, bullet.data.accel);
        }
    }
}
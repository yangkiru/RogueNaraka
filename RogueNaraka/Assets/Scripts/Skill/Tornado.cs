using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.BulletScripts;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.SkillScripts
{
    public class Tornado : Skill
    {
        public override void Use(Vector3 mp)
        {
            ShootTornado(mp);
        }

        void ShootTornado(Vector3 mp)
        {
            BulletData data = (BulletData)GameDatabase.instance.bullets[this.data.bulletIds[0]].Clone();

            data.limitTime = GetValue(Value.Time).value;
            data.worldSpeed = Mathf.Min(6.5f, GetValue(Value.Accel).value);
            data.dmg = GetValue(Value.Damage).value;
            data.disapearDuration = 0.5f;
            data.disapearStartTime = data.limitTime - 0.5f;

            Unit player = BoardManager.instance.player;
            Bullet tornado = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
            tornado.Init(player, data);
            tornado.Spawn(player.cachedTransform.position);
            Vector3 dir = mp - player.cachedTransform.position;
            tornado.shootable.Shoot(dir, Vector3.zero, data.localSpeed, data.worldSpeed, data.localAccel, data.worldAccel, false);

            //Unit soldier = BoardManager.instance.unitPool.DequeueObjectPool().GetComponent<Unit>();
            //UnitData unitData = (UnitData)GameDatabase.instance.spawnables[data.unitIds[0]].Clone();
            //unitData.stat.dmg = BoardManager.instance.player.stat.GetCurrent(STAT.TEC);
            //unitData.stat.hp = GetValue(Value.Hp).value;
            //unitData.stat.currentHp = unitData.stat.hp;
            //unitData.limitTime = GetValue(Value.Time).value;
            //soldier.Init(unitData);
            //soldier.Spawn(mp);
            //soldier.collider.isTrigger = true;
        }
    }
}
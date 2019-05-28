using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.SkillScripts
{
    public class ScarecrowSoldier : Skill
    {
        public override void Use(ref Vector3 mp)
        {
            SpawnScarecrowSoldier(ref mp);
        }

        void SpawnScarecrowSoldier(ref Vector3 mp)
        {
            Unit soldier = BoardManager.instance.unitPool.DequeueObjectPool().GetComponent<Unit>();
            UnitData unitData = (UnitData)GameDatabase.instance.spawnables[data.unitIds[0]].Clone();
            unitData.stat.dmg = BoardManager.instance.player.stat.GetCurrent(STAT.TEC);
            unitData.stat.hp = GetValue(Value.Hp).value;
            unitData.stat.currentHp = unitData.stat.hp;
            unitData.limitTime = GetValue(Value.Time).value;
            soldier.Init(unitData);
            soldier.Spawn(mp);
            soldier.collider.isTrigger = true;
        }
    }
}
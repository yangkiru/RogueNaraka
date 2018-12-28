﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.SkillScripts
{
    public class ScarecrowSoldier : Skill
    {
        public override void Use(Vector3 mp)
        {
            SpawnScarecrowSoldier(mp);
        }

        void SpawnScarecrowSoldier(Vector3 mp)
        {
            Unit soldier = BoardManager.instance.unitPool.DequeueObjectPool().GetComponent<Unit>();
            UnitData unitData = (UnitData)GameDatabase.instance.spawnables[data.unitIds[0]].Clone();
            unitData.stat.dmg = BoardManager.instance.player.stat.GetCurrent(STAT.TEC);
            unitData.stat.hp = GetValue(Value.Hp).value;
            soldier.Init(unitData);
            soldier.Spawn(mp);
        }
    }
}
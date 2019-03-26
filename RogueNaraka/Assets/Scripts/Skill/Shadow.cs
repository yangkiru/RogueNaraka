using RogueNaraka.UnitScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.SkillScripts
{
    public class Shadow : Skill
    {
        public override void Use(Vector3 mp)
        {
            SpawnShadow();
        }

        void SpawnShadow()
        {
            Unit player = BoardManager.instance.player;
            Unit shadow = BoardManager.instance.unitPool.DequeueObjectPool().GetComponent<Unit>();
            UnitData unitData = (UnitData)GameDatabase.instance.spawnables[data.unitIds[0]].Clone();
            unitData.weapon = player.data.weapon;
            unitData.stat = (Stat)BoardManager.instance.player.stat.Clone();
            unitData.stat.dmg *= 0.5f;
            unitData.stat.dmgTemp *= 0.5f;
            unitData.limitTime = GetValue(Value.Time).value;
            shadow.Init(unitData);
            player.followable.AddFollower(shadow);
            shadow.collider.enabled = false;
            shadow.targetable.SetTargetable(false);

            shadow.Spawn(player.cachedTransform.position);
        }
    }
}
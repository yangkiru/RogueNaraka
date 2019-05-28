using RogueNaraka.UnitScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.SkillScripts
{
    public class Shadow : Skill
    {
        public override void Use(ref Vector3 mp)
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
            Vector3 pos;
            player.followable.AddFollower(shadow);
            if (player.followable.Followers.Last.Previous != null)
                pos = player.followable.Followers.Last.Previous.Value.cachedTransform.position;
            else
                pos = player.cachedTransform.position;
            shadow.collider.enabled = false;
            shadow.targetable.IsTargetable = false;

            shadow.Spawn(pos);
        }
    }
}
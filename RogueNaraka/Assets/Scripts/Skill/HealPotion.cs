using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.BulletScripts;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.SkillScripts
{
    public class HealPotion : Skill
    {
        public override void Use(ref Vector3 mp)
        {
            Heal(ref mp);
            SelfDestroy();
        }

        void Heal(ref Vector3 mp)
        {
            Unit player = BoardManager.instance.player;
            player.hpable.Heal(player.hpable.maxHp);
            player.effectable.AddEffect(data.effects[0]);
        }
    }
}
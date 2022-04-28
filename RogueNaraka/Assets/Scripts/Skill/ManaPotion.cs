using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.BulletScripts;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.SkillScripts
{
    public class ManaPotion : Skill
    {
        public override void Use(ref Vector3 mp)
        {
            Heal(ref mp);
            SelfDestroy();
        }

        void Heal(ref Vector3 mp)
        {
            Unit player = BoardManager.instance.player;
            player.mpable.Heal(player.mpable.maxMp);
            player.effectable.AddEffect(data.effects[0]);
        }
    }
}
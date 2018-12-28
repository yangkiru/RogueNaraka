using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.BulletScripts;

namespace RogueNaraka.SkillScripts
{
    public class Genesis : Skill
    {
        public override void Use(Vector3 mp)
        {
            SpawnGenesis(mp);
        }

        void SpawnGenesis(Vector3 mp)
        {
            Bullet beam = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
            BulletData beamData = (BulletData)(GameDatabase.instance.bullets[data.bulletIds[0]].Clone());

            beamData.dmg = GetValue(Value.Damage).value;
            beam.Init(BoardManager.instance.player, beamData);

            beam.Spawn(mp);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.BulletScripts;

namespace RogueNaraka.SkillScripts
{
    public class IceBreak : Skill
    {
        public override void Use(ref Vector3 mp)
        {
            SpawnIce(ref mp);
        }

        void SpawnIce(ref Vector3 mp)
        {
            Bullet ice = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
            BulletData iceData = (BulletData)(GameDatabase.instance.bullets[data.bulletIds[0]].Clone());
            iceData.limitTime = Mathf.Min(iceData.limitTime + GetValue(Value.Time).value, 4);
            iceData.GetEffect(EFFECT.Ice).value += GetEffect(EFFECT.Ice).value;
            iceData.disapearStartTime = iceData.limitTime * 0.5f;
            iceData.disapearDuration = iceData.disapearStartTime;

            ice.Init(BoardManager.instance.player, iceData);
            ice.Spawn(mp);
            //ice.Spawn(BoardManager.instance.player, iceData, mp);
        }
    }
}
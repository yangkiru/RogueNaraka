using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.BulletScripts;

namespace RogueNaraka.SkillScripts
{
    public class IceBreak : Skill
    {
        public override void Use(Vector3 mp)
        {
            SpawnIce(mp);
        }

        void SpawnIce(Vector3 mp)
        {
            Bullet ice = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
            BulletData newData = (BulletData)(GameDatabase.instance.bullets[data.bulletIds[0]].Clone());
            newData.limitTime += GetValue(SKILL_VALUE.TIME).value;
            newData.GetEffect(EFFECT.ICE).value += GetEffect(EFFECT.ICE).value;
            ice.Init(BoardManager.instance.player, newData);
            ice.Spawn(mp);
            float time = ice.data.limitTime / 2;
            ice.disapearable.StartCoroutine(ice.disapearable.Disapear(time, time));
        }
    }
}
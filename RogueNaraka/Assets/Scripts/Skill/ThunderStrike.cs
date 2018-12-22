using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.BulletScripts;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.SkillScripts
{
    public class ThunderStrike : Skill
    {
        public override void Use(Vector3 mp)
        {
            StartCoroutine(SpawnThunder(mp));
        }

        IEnumerator SpawnThunder(Vector3 mp)
        {
            for (int i = 0; i < GetValue(SKILL_VALUE.AMOUNT).value; i++)
            {
                Vector2 rnd = new Vector2(Random.Range(-data.size, data.size), Random.Range(-data.size, data.size));
                Bullet thunder = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
                int rndDirection = Random.Range(0, 2);
                thunder.Init(BoardManager.instance.player, GameDatabase.instance.bullets[data.bulletIds[rndDirection]]);
                float rndAngle = Random.Range(0, 360);
                thunder.transform.rotation = Quaternion.Euler(0, 0, rndAngle);
                thunder.Spawn((Vector2)mp + rnd);
                float delay = data.values[1].value > 0 ? data.values[1].value : 0;

                yield return new WaitForSeconds(delay);
            }
        }
    }
}
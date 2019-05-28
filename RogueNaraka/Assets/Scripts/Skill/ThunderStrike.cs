using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.BulletScripts;
using RogueNaraka.UnitScripts;
using RogueNaraka.TimeScripts;

namespace RogueNaraka.SkillScripts
{
    public class ThunderStrike : Skill
    {
        public override void Use(ref Vector3 mp)
        {
            StartCoroutine(SpawnThunder(mp));
        }

        IEnumerator SpawnThunder(Vector3 mp)
        {
            WaitForFixedUpdate wait = new WaitForFixedUpdate();
            float amount = GetValue(Value.Amount).value;
            float size = Mathf.Min(data.size, 3.6f);
            //Debug.Log("Thunder" + amount);
            float delay = 0.05f;
            do
            {
                int rnd = Mathf.Clamp(Random.Range((int)(amount*0.05f), (int)(amount*0.1f)), 4,(int)amount);
                for (int j = 0; j < rnd; j++)
                {
                    Vector2 rndVec = Random.insideUnitCircle * data.size;
                    Bullet thunder = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
                    int rndDirection = Random.Range(0, 2);
                    thunder.Init(BoardManager.instance.player, GameDatabase.instance.bullets[data.bulletIds[rndDirection]]);
                    float rndAngle = Random.Range(0, 360);
                    thunder.transform.rotation = Quaternion.Euler(0, 0, rndAngle);
                    thunder.Spawn((Vector2)mp + rndVec);
                }
                amount -= rnd;

                float time = delay;
                do
                {
                    yield return wait;
                    time -= TimeManager.Instance.FixedDeltaTime;
                } while (time > 0);

            } while (amount > 0);
            //for (int i = 0; i < 10; i++)
            //{
            //    for (int j = 0; j < amount; j++)
            //    {
            //        Vector2 rnd = Random.insideUnitCircle * data.size;
            //        Bullet thunder = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
            //        int rndDirection = Random.Range(0, 2);
            //        thunder.Init(BoardManager.instance.player, GameDatabase.instance.bullets[data.bulletIds[rndDirection]]);
            //        float rndAngle = Random.Range(0, 360);
            //        thunder.transform.rotation = Quaternion.Euler(0, 0, rndAngle);
            //        thunder.Spawn((Vector2)mp + rnd);
            //    }

            //    float time = delay;
            //    do
            //    {
            //        yield return null;
            //        time -= Time.deltaTime;
            //    } while (time > 0);
            //}
        }
    }
}
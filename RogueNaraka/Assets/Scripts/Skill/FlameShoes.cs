using UnityEngine;
using System.Collections;
using RogueNaraka.UnitScripts;
using RogueNaraka.BulletScripts;
using RogueNaraka.TimeScripts;

namespace RogueNaraka.SkillScripts
{
    public class FlameShoes : Skill
    {
        public override void Use(ref Vector3 mp)
        {
            Run(ref mp);
        }

        void Run(ref Vector3 mp)
        {
            Unit player = BoardManager.instance.player;
            player.autoMoveable.enabled = false;
            player.attackable.enabled = false;
            player.moveable.Stop();
            player.rigid.AddForce((mp - player.transform.position).normalized * Vector2.Distance(mp, player.transform.position) * 7);
            StartCoroutine(CheckEnd(mp));
        }

        IEnumerator CheckEnd(Vector2 mp)
        {
            WaitForFixedUpdate wait = new WaitForFixedUpdate();
            Unit player = BoardManager.instance.player;
            yield return wait;
            float remain = player.rigid.velocity.sqrMagnitude;
            float before;
            float after = remain;

            float flameDelay = GetValue(Value.Delay).value;
            float flameTime = flameDelay;
            int amount = (int)GetValue(Value.Amount).value;

            BulletData flameData = (BulletData)GameDatabase.instance.bullets[data.bulletIds[0]].Clone();
            flameData.GetEffect(EFFECT.Fire).value = GetValue(Value.Fire).value * player.stat.GetCurrent(STAT.TEC);

            do
            {
                before = after;
                yield return wait;
                after = player.rigid.velocity.sqrMagnitude;
                float reduce = before - after;
                if (reduce > 0)
                {
                    remain -= reduce;
                }

                flameTime -= TimeManager.Instance.FixedDeltaTime;

                if (flameTime <= 0)
                {
                    for (int i = 0; i < amount; i++)
                    {
                        Bullet flame = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();

                        flame.Init(player, flameData);
                        flame.Spawn((Vector2)player.transform.position + new Vector2(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f)));
                        //Vector2 vec = (Vector2)player.transform.position + new Vector2(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
                        //flame.Spawn(player, flameData, vec);
                    }
                    flameTime = flameDelay;
                }
            } while (remain > 3);
            OnRunEnd();
        }

        void OnRunEnd()
        {
            Unit player = BoardManager.instance.player;
            player.autoMoveable.enabled = true;
            player.attackable.enabled = true;
        }
    }
}
using UnityEngine;
using System.Collections;
using RogueNaraka.UnitScripts;
using RogueNaraka.BulletScripts;

namespace RogueNaraka.SkillScripts
{
    public class FlameShoes : Skill
    {
        bool isAttackable;
        public override void Use(Vector3 mp)
        {
            Run(mp);
        }

        void Run(Vector3 mp)
        {
            Unit player = BoardManager.instance.player;
            player.autoMoveable.enabled = false;
            if (player.attackable.enabled)
            {
                isAttackable = true;
                player.attackable.enabled = false;
            }
            player.moveable.agent.Stop();
            player.rigid.AddForce((mp - player.transform.position).normalized * Vector2.Distance(mp, player.transform.position) * 7);
            StartCoroutine(CheckEnd(mp));
        }

        IEnumerator CheckEnd(Vector2 mp)
        {
            Unit player = BoardManager.instance.player;
            float bestVelocity = player.rigid.velocity.sqrMagnitude;
            float currentVelocity = bestVelocity;

            float flameDelay = GetValue(Value.Delay).value;
            float flameTime = flameDelay;
            int amount = (int)GetValue(Value.Amount).value;

            BulletData flameData = (BulletData)GameDatabase.instance.bullets[data.bulletIds[0]].Clone();
            flameData.GetEffect(EFFECT.Fire).value += GetValue(Value.Fire).value;

            while(bestVelocity == currentVelocity || bestVelocity * 0.1f < currentVelocity)
            {
                yield return null;

                currentVelocity = player.rigid.velocity.sqrMagnitude;
                if (bestVelocity < currentVelocity)
                    bestVelocity = currentVelocity;
                flameTime -= Time.deltaTime;

                if (flameTime <= 0)
                {
                    for (int i = 0; i < amount; i++)
                    {
                        Bullet flame = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();

                        flame.Init(player, flameData);
                        flame.Spawn((Vector2)player.transform.position + new Vector2(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f)));
                        //Vector2 vec = (Vector2)player.transform.position + new Vector2(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
                        //flame.Spawn(player, flameData, vec);
                        flameTime = flameDelay;
                    }
                }
            } 
            OnRunEnd();
        }

        void OnRunEnd()
        {
            Unit player = BoardManager.instance.player;
            if(isAttackable)
                player.autoMoveable.enabled = true;
            player.attackable.enabled = true;
        }
    }
}
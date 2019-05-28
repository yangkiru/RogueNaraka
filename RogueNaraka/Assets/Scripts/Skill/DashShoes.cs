using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;
using RogueNaraka.EffectScripts;
using RogueNaraka.BulletScripts;

namespace RogueNaraka.SkillScripts
{
    public class DashShoes : Skill
    {
        Effect accel;
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
            float delay = GetValue(Value.Delay).value;
            float t = delay;

            //for (int i = 0; i < 10; i++)
            //{
            //    Vector2 rnd = Random.insideUnitCircle * 0.35f;
            //    Bullet dust = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
            //    dust.Init(BoardManager.instance.player, GameDatabase.instance.bullets[data.bulletIds[0]]);
            //    dust.Spawn(BoardManager.instance.player.cachedTransform.position + (Vector3)rnd);
            //    dust.cachedTransform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            //}
            do
            {
                before = after;
                yield return wait;
                //t -= Time.fixedDeltaTime;
                //if (t <= 0)
                //{
                //    t = delay;
                //    Bullet dust = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
                //    dust.Init(BoardManager.instance.player, GameDatabase.instance.bullets[data.bulletIds[0]]);
                //    dust.Spawn(BoardManager.instance.player.cachedTransform.position);
                //}

                after = player.rigid.velocity.sqrMagnitude;
                float reduce = before - after;
                if (reduce > 0)
                {
                    remain -= reduce;
                }
            } while (remain > 3);
            OnRunEnd();
        }

        void OnRunEnd()
        {
            Debug.Log("RunEnd");
            Unit player = BoardManager.instance.player;
            player.autoMoveable.enabled = true;
            player.attackable.enabled = true;
        }

        //void Run(Vector3 mp)
        //{
        //    Unit player = BoardManager.instance.player;
        //    player.autoMoveable.enabled = false;
        //    player.attackable.enabled = false;
        //    player.moveable.Stop();
        //    player.moveable.AccelerationRate = (player.data.accelerationRate == 0 ? 0.5f : player.data.accelerationRate) * 10;
        //    player.moveable.DecelerationRate = 0.9f;
        //    player.moveable.SetDestination(mp, OnRunEnd);
        //    float additionalSpeed = Mathf.Max(0, data.size - 8) * 0.2f;
        //    EffectData accelData = (EffectData)data.effects[0].Clone();
        //    accelData.value += additionalSpeed;
        //    accel = player.effectable.AddEffect(accelData);
        //    //    player.rigid.AddForce((mp - player.transform.position).normalized * Vector2.Distance(mp, player.transform.position) * 7);
        //    //    StartCoroutine(CheckEnd(mp));
        //}

        //void OnRunEnd(bool isArrive)
        //{
        //    Unit player = BoardManager.instance.player;
        //    player.autoMoveable.enabled = true;
        //    player.autoMoveable.leftDelay = 0;
        //    player.moveable.AccelerationRate = player.data.accelerationRate == 0 ? 0.5f : player.moveable.AccelerationRate = player.data.accelerationRate;
        //    player.moveable.DecelerationRate = player.data.decelerationRate == 0 ? 1f : player.data.decelerationRate;

        //    player.attackable.enabled = true;

        //    if (accel != null)
        //        accel.Destroy();
        //}
    }
}
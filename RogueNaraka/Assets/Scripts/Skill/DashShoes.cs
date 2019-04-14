using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;
using RogueNaraka.EffectScripts;

namespace RogueNaraka.SkillScripts
{
    public class DashShoes : Skill
    {
        Effect accel;
        public override void Use(Vector3 mp)
        {
            Run(mp);
        }

        void Run(Vector3 mp)
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
            Unit player = BoardManager.instance.player;
            yield return new WaitForFixedUpdate();
            float remain = player.rigid.velocity.sqrMagnitude;
            float before;
            float after = remain;
            do
            {
                before = after;
                yield return new WaitForFixedUpdate();
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
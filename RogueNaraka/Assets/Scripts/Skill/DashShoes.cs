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

        //void Run(Vector3 mp)
        //{
        //    Unit player = BoardManager.instance.player;
        //    player.autoMoveable.enabled = false;
        //    player.attackable.enabled = false;
        //    player.moveable.Stop();
        //    player.rigid.AddForce((mp - player.transform.position).normalized * Vector2.Distance(mp, player.transform.position) * 7);
        //    StartCoroutine(CheckEnd(mp));
        //}

        void Run(Vector3 mp)
        {
            Unit player = BoardManager.instance.player;
            player.autoMoveable.enabled = false;
            player.attackable.enabled = false;
            player.moveable.Stop();
            player.moveable.DecelerationRate = 10;
            player.moveable.SetDestination(mp, OnRunEnd);
            float additionalSpeed = Mathf.Max(0, data.size - 8) * 0.2f;
            EffectData accelData = (EffectData)data.effects[0].Clone();
            accelData.value += additionalSpeed;
            accel = player.effectable.AddEffect(accelData);
            //    player.rigid.AddForce((mp - player.transform.position).normalized * Vector2.Distance(mp, player.transform.position) * 7);
            //    StartCoroutine(CheckEnd(mp));
        }
        
        void OnRunEnd(bool isArrive)
        {
            Unit player = BoardManager.instance.player;
            player.autoMoveable.enabled = true;
            player.autoMoveable.leftDelay = 0;
            player.moveable.DecelerationRate = GameDatabase.instance.playerBase.decelerationRate;
            player.moveable.DecelerationRate = player.moveable.DecelerationRate == 0 ? 1 : player.moveable.DecelerationRate;

            player.attackable.enabled = true;
            
            if (accel != null)
                accel.Destroy();
        }

        //IEnumerator CheckEnd(Vector2 mp)
        //{
        //    Unit player = BoardManager.instance.player;
        //    float currentVelocity = player.rigid.velocity.sqrMagnitude;
        //    float lastVelocity = currentVelocity;
        //    while (currentVelocity >= lastVelocity)
        //    {
        //        yield return null;
        //        lastVelocity = currentVelocity;
        //        currentVelocity = player.rigid.velocity.sqrMagnitude;
        //    }
        //    OnRunEnd();
        //}

        //void OnRunEnd()
        //{
        //    Unit player = BoardManager.instance.player;
        //    player.autoMoveable.enabled = true;
        //    player.attackable.enabled = true;
        //}
    }
}
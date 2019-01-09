using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.SkillScripts
{
    public class DashShoes : Skill
    {
        public override void Use(Vector3 mp)
        {
            Run(mp);
        }

        void Run(Vector3 mp)
        {
            Unit player = BoardManager.instance.player;
            player.autoMoveable.enabled = false;
            player.attackable.enabled = false;
            player.moveable.agent.Stop();
            player.rigid.AddForce((mp - player.transform.position).normalized * Vector2.Distance(mp, player.transform.position) * 7);
            StartCoroutine(CheckEnd(mp));
        }

        IEnumerator CheckEnd(Vector2 mp)
        {
            Unit player = BoardManager.instance.player;
            float currentVelocity = player.rigid.velocity.sqrMagnitude;
            float lastVelocity = currentVelocity;
            while (currentVelocity >= lastVelocity)
            {
                yield return null;
                lastVelocity = currentVelocity;
                currentVelocity = player.rigid.velocity.sqrMagnitude;
            }
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
﻿using UnityEngine;
using System.Collections;
using RogueNaraka.UnitScripts;
using RogueNaraka.BulletScripts;
using RogueNaraka.TimeScripts;

namespace RogueNaraka.SkillScripts
{
    public class JumpingShoes : Skill
    {
        public override void Use(Vector3 mp)
        {
            Jump(mp);
        }

        void Jump(Vector3 mp)
        {
            Unit player = BoardManager.instance.player;
            player.collider.enabled = false;
            player.autoMoveable.enabled = false;
            player.attackable.enabled = false;
            player.targetable.IsTargetable = false;
            player.moveable.Stop();

            StartCoroutine(JumpCorou(mp, 0.75f));
        }

        IEnumerator JumpCorou(Vector2 mp, float time)
        {
            Unit player = BoardManager.instance.player;

            do
            {
                yield return null;
                time -= TimeManager.Instance.DeltaTime;
            } while (time > 0);

            player.cachedTransform.position = mp;
            BulletData crashData = GameDatabase.instance.bullets[data.bulletIds[0]];
            Bullet crashBullet = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();

            crashBullet.Spawn(player, crashData, mp);

            OnJumpEnd();
        }

        void OnJumpEnd()
        {
            Unit player = BoardManager.instance.player;
            player.collider.enabled = true;
            player.autoMoveable.enabled = true;
            player.attackable.enabled = true;
            player.targetable.IsTargetable = true;
        }
    }
}
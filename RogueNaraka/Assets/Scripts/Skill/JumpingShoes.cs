using UnityEngine;
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

            player.rigid.AddForce(new Vector2(0, 75));
            player.shadow.enabled = false;
            Transform shadowTransform = player.shadow.transform;
            shadowTransform.SetParent(null);
            Vector3 shadowScale = shadowTransform.localScale;
            do
            {
                yield return null;
                time -= TimeManager.Instance.DeltaTime;
                shadowScale.x -= TimeManager.Instance.DeltaTime;
                shadowScale.y -= TimeManager.Instance.DeltaTime;
                shadowTransform.localScale = shadowScale;
                if (player.cachedTransform.position.y > BoardManager.instance.boardRange[1].y - 1)
                    player.renderer.enabled = false;
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
            player.renderer.enabled = true;
            
            Transform shadowTransform = player.shadow.transform;
            shadowTransform.localScale = Vector3.one;
            shadowTransform.SetParent(player.cachedTransform);
            player.shadow.enabled = true;
            player.rigid.velocity = Vector2.zero;
            player.collider.enabled = true;
            player.autoMoveable.enabled = true;
            player.attackable.enabled = true;
            player.targetable.IsTargetable = true;
        }
    }
}
using UnityEngine;
using System.Collections;
using RogueNaraka.UnitScripts;
using RogueNaraka.BulletScripts;
using RogueNaraka.TimeScripts;

namespace RogueNaraka.SkillScripts
{
    public class JumpingShoes : Skill
    {
        public static int jumping = 0;
        public override void Use(Vector3 mp)
        {
            Jump(mp);
        }

        void Jump(Vector3 mp)
        {
            StartCoroutine(JumpCorou(mp, 0.75f));
        }

        IEnumerator JumpCorou(Vector2 mp, float time)
        {
            float tt = time;
            Unit player = BoardManager.instance.player;

            while (jumping > 0)
            {
                data.coolTimeLeft = data.coolTime;
                yield return null;
                if (jumping <= 0)
                {
                    float t = 0.25f;
                    do
                    {
                        yield return null;
                        t -= TimeManager.Instance.DeltaTime;
                    } while (t > 0);
                }
            }

            player.collider.enabled = false;
            player.autoMoveable.enabled = false;
            player.attackable.enabled = false;
            player.targetable.IsTargetable = false;
            player.moveable.Stop();

            jumping++;

            player.rigid.AddForce(new Vector2(0, 75));
            player.shadow.enabled = false;
            Transform shadowTransform = player.shadow.transform;
            shadowTransform.SetParent(null);
            Vector3 shadowScale = shadowTransform.localScale;
            player.followable.isFollow = false;


            do
            {
                yield return null;
                time -= TimeManager.Instance.DeltaTime;
                float amount = TimeManager.Instance.DeltaTime * tt;
                shadowScale.x -= amount;
                shadowScale.y -= amount;
                shadowTransform.localScale = shadowScale;
                if (player.cachedTransform.position.y > BoardManager.instance.boardRange[1].y - 1)
                {
                    player.renderer.enabled = false;
                    player.cachedTransform.position = mp;
                    player.rigid.velocity = Vector2.zero;
                }
            } while (time > 0);

            BulletData crashData = GameDatabase.instance.bullets[data.bulletIds[0]];
            Bullet crashBullet = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();

            crashBullet.Spawn(player, crashData, mp);

            OnJumpEnd();
        }

        void OnJumpEnd()
        {
            jumping--;
            Unit player = BoardManager.instance.player;
            player.renderer.enabled = true;
            player.followable.isFollow = true;
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
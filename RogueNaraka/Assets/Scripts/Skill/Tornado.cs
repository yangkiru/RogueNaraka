using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.BulletScripts;
using RogueNaraka.UnitScripts;
using RogueNaraka.TimeScripts;

namespace RogueNaraka.SkillScripts
{
    public class Tornado : Skill
    {
        public override void Use(ref Vector3 mp)
        {
            ShootTornado(ref mp);
        }

        void ShootTornado(ref Vector3 mp)
        {
            BulletData data = (BulletData)GameDatabase.instance.bullets[this.data.bulletIds[0]].Clone();

            data.limitTime = GetValue(Value.Time).value;
            data.worldSpeed = Mathf.Min(6.5f, GetValue(Value.Accel).value);
            data.dmg = GetValue(Value.Damage).value;
            data.disapearDuration = 0.5f;
            data.disapearStartTime = data.limitTime - 0.5f;

            Unit player = BoardManager.instance.player;
            Bullet tornado = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
            tornado.Init(player, data);
            tornado.hitable.OnDamage += OnTornadoHit;
            tornado.Spawn(player.cachedTransform.position);
            Vector3 dir = mp - player.cachedTransform.position;
            tornado.shootable.Shoot(dir, Vector3.zero, data.localSpeed, data.worldSpeed, data.localAccel, data.worldAccel, false);
            StartCoroutine("TornadoDust", tornado);
        }

        private void OnTornadoHit(Bullet from, Unit to)
        {
            BulletData dustData = GameDatabase.instance.bullets[data.bulletIds[1]];
            Bullet dust = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
            dust.Init(from.ownerable.unit, dustData);
            Vector3 pos = (from.cachedTransform.position - to.cachedTransform.position) * 0.25f;
            dust.Spawn(to.cachedTransform.position + pos);
            dust.cachedTransform.rotation = MathHelpers.GetRandomAngle(0, 360);
        }

        IEnumerator TornadoDust(Bullet bullet)
        {
            WaitForFixedUpdate wait = new WaitForFixedUpdate();
            BulletData dustData = GameDatabase.instance.bullets[data.bulletIds[2]];
            float t = 0.2f;
            float tt = t;
            int id = AudioManager.instance.PlaySFXLoop("tornado", 1, 4);
            do
            {
                yield return wait;
                t -= TimeManager.Instance.FixedDeltaTime;
                if (t < 0)
                {
                    t = tt;
                    for (int i = 0; i < 3; i++)
                    {
                        Bullet dust = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
                        dust.Init(bullet.ownerable.unit, dustData);
                        Vector2 offset = new Vector2(i == 0 ? -0.2f : i == 1 ? 0.2f : 0, -0.5f);//Random.Range(-0.75f, 0.75f)
                        dust.Spawn(bullet.cachedTransform.position + (Vector3)offset);
                        dust.cachedTransform.rotation = MathHelpers.GetRandomAngle();
                        dust.shootable.Shoot(i == 0 ? Vector2.left : i == 1 ? Vector2.right : Vector2.zero, Vector3.zero, 0, 0.5f, 0, 0, false);
                    }
                }
            } while (bullet.gameObject.activeSelf);
            AudioManager.instance.FadeOutSFX(id, 1);
        }

        private void OnDisable()
        {
            StopCoroutine("TornadoDust");
        }
    }
}
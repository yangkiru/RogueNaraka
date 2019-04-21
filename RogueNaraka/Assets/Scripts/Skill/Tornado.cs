using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.BulletScripts;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.SkillScripts
{
    public class Tornado : Skill
    {
        public override void Use(Vector3 mp)
        {
            ShootTornado(mp);
        }

        void ShootTornado(Vector3 mp)
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
        }

        private void OnTornadoHit(Bullet from, Unit to)
        {
            BulletData dustData = GameDatabase.instance.bullets[data.bulletIds[1]];
            Bullet dust = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
            dust.Init(from.ownerable.unit, dustData);
            dust.Spawn(to.cachedTransform.position);
            dust.cachedTransform.rotation = MathHelpers.GetRandomAngle(0, 360);
        }
    }
}
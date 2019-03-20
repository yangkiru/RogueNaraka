using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.BulletScripts;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.SkillScripts
{
    public class BloodSwamp : Skill
    {
        public override void Use(Vector3 mp)
        {
            SpawnBloodSwamp(mp);
        }

        void SpawnBloodSwamp(Vector3 mp)
        {
            for (int i = 0; i < GetValue(Value.Amount).value; i++)//values[1] == blood spawn amount
            {
                float rndAngle = Random.Range(0, 360);
                Vector2 rndPos = new Vector2(Random.Range(-data.size + 1.5f, data.size - 1.5f), Random.Range(-data.size + 1.5f, data.size - 1.5f));
                Bullet blood = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
                BulletData newData = (BulletData)(GameDatabase.instance.bullets[data.bulletIds[0]].Clone());
                blood.Init(BoardManager.instance.player, newData);
                blood.hitable.OnDamage += SpawnBloodBubble;
                float time = blood.data.limitTime / 2;
                blood.transform.rotation = Quaternion.Euler(0, 0, rndAngle);
                blood.Spawn((Vector2)mp + rndPos);
            }
        }

        void SpawnBloodBubble(Bullet from, Unit to)
        {
            Bullet bubble = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
            Vector2 rndPos = new Vector2(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f));
            BulletData bubbleData = (BulletData)GameDatabase.instance.bullets[data.bulletIds[1]].Clone();
            bubble.Init(to, bubbleData);
            bubble.hitable.OnDamage += OnBloodBubbleHit;
            bubble.Spawn((Vector2)to.transform.position + rndPos);
            bubble.shootable.Shoot(from.transform.position - to.transform.position, Vector3.zero, bubbleData.localSpeed, bubbleData.worldSpeed, bubbleData.localAccel, bubbleData.worldAccel);
        }

        void OnBloodBubbleHit(Bullet from, Unit to)
        {
            float lifeSteal = GetValue(Value.LifeSteal).value;
            Unit fromUnit = from.ownerable.unit;
            float amount = lifeSteal;

            if (!from.ownerable.unit.deathable.isDeath)
            {
                fromUnit.damageable.Damage(amount * 0.03f * BoardManager.instance.player.stat.GetCurrent(STAT.TEC));
                if (!to.deathable.isDeath)
                    to.hpable.Heal(amount * 0.025f);
            }
        }
    }
}
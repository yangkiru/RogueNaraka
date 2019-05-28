using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.BulletScripts;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.SkillScripts
{
    public class BloodSwamp : Skill
    {
        public override void Use(ref Vector3 mp)
        {
            SpawnBloodSwamp(ref mp);
        }

        void SpawnBloodSwamp(ref Vector3 mp)
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

        void SpawnBloodBubble(Bullet from, Unit to)
        {
            Bullet bubble = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
            BulletData bubbleData = (BulletData)GameDatabase.instance.bullets[data.bulletIds[1]].Clone();
            bubble.Init(to, bubbleData);
            bubble.hitable.OnDamage += OnBloodBubbleHit;
            bubble.Spawn((Vector2)to.transform.position);
            bubble.shootable.Shoot(from.transform.position - to.transform.position, Vector3.zero, bubbleData.localSpeed, bubbleData.worldSpeed, bubbleData.localAccel, bubbleData.worldAccel);
        }

        void OnBloodBubbleHit(Bullet from, Unit to)
        {
            float lifeSteal = GetValue(Value.LifeSteal).value;
            float hpRegenDmg = GetValue(Value.HpRegen).value;
            Unit fromUnit = from.ownerable.unit;

            if (!from.ownerable.unit.deathable.isDeath)
            {
                float result = BoardManager.instance.player.stat.GetCurrent(STAT.HR) * hpRegenDmg + lifeSteal;
                fromUnit.damageable.Damage(result);
                if (!to.deathable.isDeath)
                    to.hpable.Heal(result);
            }
        }
    }
}
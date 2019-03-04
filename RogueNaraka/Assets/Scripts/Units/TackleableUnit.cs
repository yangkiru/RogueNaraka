﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RogueNaraka.BulletScripts;
using RogueNaraka.UnitScripts;

namespace RogueNaraka
{

    public class TackleableUnit : MonoBehaviour
    {
        const int tackleBulletID = 3;

        public Unit unit;

        public bool isTackle { get { return _isTackle; } set { _isTackle = value; } }

        bool _isTackle;

        UnitData data;

        List<CorouUnit> hits = new List<CorouUnit>();

        public void Init(UnitData data)
        {
            if (data.tackleSize > 0 && data.tackleDamage != 0)
            {
                _isTackle = true;
                this.data = data;
                this.enabled = true;
            }
            else
            {
                _isTackle = false;
                this.data = null;
                this.enabled = false;
            }
        }

        private void OnEnable()
        {
            StartCoroutine(RepeatCorou());
        }

        //private void OnTriggerEnter2D(Collider2D collision)
        //{
        //    Unit unit = collision.GetComponent<Unit>();
        //    Debug.Log("OnTriggerEnter" + name + collision.name);
        //    if (unit)
        //    {
        //        CorouUnit corouUnit = new CorouUnit();
        //        hits.Add(corouUnit);
        //        corouUnit.corou = DamageCorou(corouUnit);
        //        corouUnit.unit = unit;
        //        StartCoroutine(corouUnit.corou);

        //    }
        //}

        //private void OnTriggerExit2D(Collider2D collision)
        //{
        //    Unit unit = collision.GetComponent<Unit>();
        //    Debug.Log("OnTriggerExit");
        //    if (unit)
        //    {
        //        for(int i = 0; i < hits.Count; i++)
        //        {
        //            if(hits[i].unit.Equals(unit))
        //            {
        //                StopCoroutine(hits[i].corou);
        //            }
        //        }
        //    }
        //}

        //IEnumerator DamageCorou(CorouUnit unit)
        //{
        //    float t = data.tackleDelay;
        //    Debug.Log("DamageCorou");
        //    while (true)
        //    {
        //        unit.unit.damageable.Damage(data.tackleDamage * data.stat.GetCurrent(STAT.DMG));
        //        Debug.Log("Damage");
        //        do
        //        {
        //            yield return null;
        //            t -= Time.deltaTime;
        //        } while (t > 0);
        //    }
        //}

        IEnumerator RepeatCorou()
        {
            while (true)
            {
                float t = data.tackleDelay;

                BulletData bulletData = (BulletData)GameDatabase.instance.bullets[tackleBulletID].Clone();
                bulletData.size = data.tackleSize;
                bulletData.dmg = data.tackleDamage;

                if (_isTackle && data != null)
                {
                    Bullet tackleBullet = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();

                    tackleBullet.Spawn(unit, bulletData, unit.cashedTransform.position);
                }

                do
                {
                    yield return null;
                    t -= Time.deltaTime;
                } while (t > 0);
            }
        }

        public class CorouUnit
        {
            public Unit unit;
            public IEnumerator corou;
        }
    }
}
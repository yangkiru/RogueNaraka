using UnityEngine;
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

        //List<CorouUnit> hits = new List<CorouUnit>();

        public void Init(UnitData data)
        {
            if (data.tackleSize > 0 && data.tackleDamage != 0 && data.tackleDelay > 0)
            {
                _isTackle = true;
                //this.enabled = true;
            }
            else
            {
                _isTackle = false;
                //this.enabled = false;
            }
        }

        public void OnSpawn()
        {
            if(_isTackle)
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
            yield return null;
            do
            {
                yield return null;
            } while (!BoardManager.instance.isReady);

            while (true)
            {
                float t = unit.data.tackleDelay;

                if(_isTackle && BoardManager.instance.isReady)
                    Tackle();

                do
                {
                    yield return null;
                    if(!unit.isStun)
                        t -= Mathf.Max(0, Time.deltaTime * (1 + unit.stat.GetCurrent(STAT.SPD) * 0.1f));
                } while (t > 0);
            }
        }

        public void Tackle()
        {
            //Debug.Log("Tackle");
            BulletData bulletData = (BulletData)GameDatabase.instance.bullets[tackleBulletID].Clone();
            bulletData.size = unit.data.tackleSize;
            bulletData.dmg = unit.data.tackleDamage;

            Bullet tackleBullet = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();

            tackleBullet.Spawn(unit, bulletData, unit.cachedTransform.position);
        }

        public class CorouUnit
        {
            public Unit unit;
            public IEnumerator corou;
        }
    }
}
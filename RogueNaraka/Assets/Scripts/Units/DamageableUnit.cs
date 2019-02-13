using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts
{
    public class DamageableUnit : MonoBehaviour
    {
        [SerializeField]
        Unit unit;

        [SerializeField]
        HpableUnit hpable;

        float damaged;

        private void Reset()
        {
            unit = GetComponent<Unit>();
            hpable = GetComponent<HpableUnit>();
        }

        void OnEnable()
        {
            damaged = 0;
            StartCoroutine(DamageCorou());
        }

        public void Damage(float amount)
        {
            if (!hpable.isInvincible) 
            {
                hpable.AddHp(-amount);
                damaged += amount;
            }
        }

        //void Update()
        //{
        //    if (time > 0)
        //        time -= Time.deltaTime;
        //    else if(damaged > 0)
        //    {
        //        Color color;
        //        if (unit.data.isFriendly)
        //            color = Color.red;
        //        else
        //            color = Color.white;
        //        PointTxtManager.instance.TxtOnHead(damaged, transform, color);
        //        damaged = 0;
        //        time = 0.1f;
        //    }
        //}

        IEnumerator DamageCorou()
        {
            while(true)
            {
                if (damaged != 0)
                {
                    float time = 0.1f;

                    Color color;
                    if (unit.data.isFriendly)
                        color = Color.red;
                    else
                        color = Color.white;
                    PointTxtManager.instance.TxtOnHead(-damaged, transform, color);
                    damaged = 0;

                    do
                    {
                        yield return null;
                        time -= Time.deltaTime;
                    } while (time > 0);
                }
                else
                {
                    yield return null;
                }
            }
        }

        void OnDisable()
        {
            if (damaged != 0)
            {
                Color color;
                if (unit.data.isFriendly)
                    color = Color.red;
                else
                    color = Color.white;
                PointTxtManager.instance.TxtOnHead(damaged, transform, color);
            }
        }
    }
}

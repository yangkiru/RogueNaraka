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
        float time;

        private void Reset()
        {
            unit = GetComponent<Unit>();
            hpable = GetComponent<HpableUnit>();
        }

        void OnEnable()
        {
            damaged = 0;
            time = 0;
        }

        public void Damage(float amount)
        {
            hpable.AddHp(-amount);
            damaged -= amount;
        }

        void Update()
        {
            time += Time.deltaTime;
            if(time >= 0.1f && damaged != 0)
            {
                Color color;
                if (unit.data.isFriendly)
                    color = Color.red;
                else
                    color = Color.white;
                PointTxtManager.instance.TxtOnHead(damaged, transform, color);
                damaged = 0;
                time = 0;
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

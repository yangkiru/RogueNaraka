using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.Bullet
{
    public abstract class Hitable : MonoBehaviour
    {
        DamageableBullet damageableBullet;
        List<int> hitList = new List<int>();
        List<Unit> hitUnitList = new List<Unit>();

        private void Awake()
        {
            damageableBullet = GetComponent<DamageableBullet>();
        }
        void Update()
        {
            GetHitUnits(hitUnitList);
            for(int i = 0; i < hitUnitList.Count; i++)
                damageableBullet.Damage(hitUnitList[i]);
        }

        public abstract void GetHitUnits(List<Unit> hitList);

        public bool CheckHitList(Unit unit)
        {
            int hashCode = unit.GetHashCode();
            if (hitList.Contains(hashCode))
                return false;
            return true;
        }

        public bool CheckHitList(int hashCode)
        {
            if (hitList.Contains(hashCode))
                return false;
            return true;
        }

        public bool AddHitList(Unit unit)
        {
            int hashCode = unit.GetHashCode();
            if(CheckHitList(hashCode))
            {
                hitList.Add(hashCode);
                return true;
            }
            return false;
        }
    }
}

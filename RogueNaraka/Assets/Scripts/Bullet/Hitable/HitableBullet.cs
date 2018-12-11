using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.BulletScripts.Hitable
{
    public abstract class HitableBullet : MonoBehaviour
    {
        DamageableBullet damageable;
        OwnerableBullet ownerable;
        List<int> hitList = new List<int>();
        List<OldUnit> hitUnitList = new List<OldUnit>();

        [SerializeField]
        protected LayerMask layerMask;
        public LayerMask wallLayerMask;

        private void Awake()
        {
            damageable = GetComponent<DamageableBullet>();
            ownerable = GetComponent<OwnerableBullet>();
        }
        void Update()
        {
            GetHitUnits(hitUnitList);
            for (int i = 0; i < hitUnitList.Count; i++)
            {
                if(damageable)
                    damageable.Damage(hitUnitList[i]);
            }
        }

        public virtual void Init(BulletData data)
        {
            layerMask = GetLayerMask();
        }

        public abstract void GetHitUnits(List<OldUnit> hitList);

        public bool CheckHitList(OldUnit unit)
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

        public bool AddHitList(OldUnit unit)
        {
            int hashCode = unit.GetHashCode();
            if(CheckHitList(hashCode))
            {
                hitList.Add(hashCode);
                return true;
            }
            return false;
        }

        protected LayerMask GetLayerMask()
        {
            if (ownerable)
                return ownerable.layerMask + wallLayerMask;
            else
                return LayerMask.GetMask("Friendly", "Enemy") + wallLayerMask;
        }
    }
}

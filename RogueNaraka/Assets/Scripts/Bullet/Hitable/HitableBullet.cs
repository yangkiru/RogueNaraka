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
        [SerializeField]
        List<int> hitList = new List<int>();
        [SerializeField]
        protected List<Unit> hitUnitList = new List<Unit>();

        [SerializeField]
        protected LayerMask layerMask;

        float delay;
        float leftDelay;

        private void Awake()
        {
            damageable = GetComponent<DamageableBullet>();
            ownerable = GetComponent<OwnerableBullet>();
        }
        void Update()
        {
            if (delay > 0)
            {
                leftDelay -= Time.deltaTime;
                return;
            }
            else
                leftDelay = delay;
            GetHitUnits();
            for (int i = 0; i < hitUnitList.Count; i++)
            {
                if(damageable)
                    damageable.Damage(hitUnitList[i]);
            }
        }

        public virtual void Init(BulletData data)
        {
            layerMask = GetLayerMask();
            delay = data.delay;
            leftDelay = 0;
        }

        public abstract void GetHitUnits();

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
            if (!unit)
                return false;
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
                return (1 << ownerable.layer) | (1 << GameDatabase.wallLayer);
            else
                return  (1 << GameDatabase.friendlyLayer) | (1 << GameDatabase.enemyLayer) | (1 << GameDatabase.wallLayer);
        }
    }
}

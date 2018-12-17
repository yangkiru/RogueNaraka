using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.BulletScripts.Hitable
{
    public abstract class HitableBullet : MonoBehaviour
    {
        [SerializeField]
        protected Bullet bullet;

        [SerializeField]
        DamageableBullet damageable;
        [SerializeField]
        OwnerableBullet ownerable;
        [SerializeField]
        List<int> hitList = new List<int>();
        [SerializeField]
        protected List<Unit> hitUnitList = new List<Unit>();

        [SerializeField]
        protected LayerMask layerMask;

        float delay;
        float leftDelay;

        protected int pierce;

        private void Reset()
        {
            bullet = GetComponent<Bullet>();
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
                damageable.Damage(hitUnitList[i]);
            }
        }

        public virtual void Init(BulletData data)
        {
            layerMask = GetLayerMask();
            delay = data.delay;
            leftDelay = 0;
            
        }

        protected abstract void GetHitUnits();

        protected void SubPierce()
        {
            if (pierce-- == 1)
                bullet.Destroy();
        }

        protected bool CheckHitList(Unit unit)
        {
            int hashCode = unit.GetHashCode();
            if (hitList.Contains(hashCode))
                return false;
            return true;
        }

        protected bool CheckHitList(int hashCode)
        {
            if (hitList.Contains(hashCode))
                return false;
            return true;
        }

        protected bool AddHitList(Unit unit)
        {
            if (!unit)
            {
                bullet.Destroy();
                return false;
            }
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
            {
                return (ownerable.layer == GameDatabase.friendlyLayer) ? GameDatabase.instance.enemyMask : GameDatabase.instance.friendlyMask;
            }
            else
                return (1 << GameDatabase.friendlyLayer) | (1 << GameDatabase.enemyLayer) | (1 << GameDatabase.wallLayer);
        }
    }
}

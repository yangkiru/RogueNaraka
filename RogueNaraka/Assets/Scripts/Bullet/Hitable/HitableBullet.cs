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
        OwnerableBullet ownerable;
        [SerializeField]
        protected List<Unit> hitList;

        [SerializeField]
        protected LayerMask layerMask;

        float delay;
        float leftDelay;
        
        [SerializeField]
        protected int pierce;

        private void Reset()
        {
            bullet = GetComponent<Bullet>();
            ownerable = GetComponent<OwnerableBullet>();
        }

        void Update()
        {
            if (leftDelay > 0)
            {
                leftDelay -= Time.deltaTime;
                return;
            }
            else
                leftDelay = delay;
            GetHitUnits();
            for (int i = 0; i < hitList.Count; i++)
            {
                Debug.Log(name + " hit " + hitList[i].name);
                bullet.damageable.Damage(hitList[i]);
            }
            hitList.Clear();
        }

        public virtual void Init(BulletData data)
        {
            layerMask = GetLayerMask();
            delay = data.delay;
            leftDelay = 0;
            pierce = data.pierce;
            hitList = new List<Unit>();
        }

        protected abstract void GetHitUnits();

        protected void SubPierce()
        {
            if (pierce-- == 1)
                bullet.Destroy();
        }

        protected bool CheckHitList(Unit unit)
        {
            for (int i = 0; i < hitList.Count; i++)
                if (hitList[i].Equals(unit))
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
            if(CheckHitList(unit))
            {
                hitList.Add(unit);
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

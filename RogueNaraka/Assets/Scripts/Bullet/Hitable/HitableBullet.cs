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
        ShakeableBullet shakeable;
        [SerializeField]
        protected List<Unit> hitList;

        [SerializeField]
        protected LayerMask layerMask;

        [SerializeField]
        float delay;
        [SerializeField]
        float leftDelay;
        
        [SerializeField]
        protected int pierce;

        public event System.Action<Bullet, Unit> OnDamage;

        private void Reset()
        {
            bullet = GetComponent<Bullet>();
            ownerable = GetComponent<OwnerableBullet>();
            shakeable = GetComponent<ShakeableBullet>();
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
                //Debug.Log(name + " hit " + hitList[i].name);
                
                for(int j = 0; j < bullet.data.effects.Length; j++)
                {
                    hitList[i].effectable.AddEffect(bullet.data.effects[j], bullet, ownerable.unit);
                }
                bullet.damageable.Damage(hitList[i], bullet.data.related);
                if(shakeable.shake.isOnHit)
                    shakeable.Shake();
                if (OnDamage != null)
                    OnDamage(bullet, hitList[i]);
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
            OnDamage = null;
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
            if (!unit && pierce >= 0)
            {
                bullet.Destroy();
                return false;
            }
            if(unit && CheckHitList(unit))
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

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
        //[SerializeField]
        //protected List<Unit> hitList = new List<Unit>();

        [SerializeField]
        protected LayerMask layerMask;

        [SerializeField]
        float delay;
        [SerializeField]
        float leftDelay;

        bool isHit;
        bool isDestroy;

        
        [SerializeField]
        protected int pierce;

        public event System.Action<Bullet, Unit> OnDamage;

        private void Reset()
        {
            bullet = GetComponent<Bullet>();
            ownerable = GetComponent<OwnerableBullet>();
            shakeable = GetComponent<ShakeableBullet>();
        }

        protected virtual void Update()
        {
            if (leftDelay > 0)
            {
                leftDelay -= Time.deltaTime;
                return;
            }
            if (isHit)
            {
                leftDelay = delay;
                isHit = false;

                OnHit();
            }
            else
            {
                OnNotHit();
            }
            //GetHitUnits();
            //for (int i = 0; i < hitList.Count; i++)
            //{
            //    //Debug.Log(name + " hit " + hitList[i].name);
                
            //    for(int j = 0; j < bullet.data.effects.Length; j++)
            //    {
            //        hitList[i].effectable.AddEffect(bullet.data.effects[j], bullet, ownerable.unit);
            //    }
            //    bullet.damageable.Damage(hitList[i], bullet.data.related);
            //    if(shakeable.shake.isOnHit)
            //        shakeable.Shake();
            //    if (OnDamage != null)
            //        OnDamage(bullet, hitList[i]);
            //}
            //hitList.Clear();
        }

        /// <summary>
        /// Call this on children
        /// </summary>
        /// <param name="coll"></param>
        protected void Hit(Collider2D coll)
        {
            if (isDestroy || leftDelay > 0)
                return;
            if ((layerMask.value & (1 << coll.gameObject.layer)) != (1 << coll.gameObject.layer))
                return;

            Unit hit = coll.GetComponent<Unit>();

            if (hit)
            {
                for (int i = 0; i < bullet.data.effects.Length; i++)
                {
                    hit.effectable.AddEffect(bullet.data.effects[i], bullet, ownerable.unit);
                }

                bullet.damageable.Damage(hit, bullet.data.related);

                if (shakeable.shake.isOnHit)
                    shakeable.Shake();

                if (OnDamage != null)
                    OnDamage(bullet, hit);
            }

            if (pierce-- == 1)
            {
                isDestroy = true;
                bullet.Destroy();
            }
            isHit = true;
        }

        protected virtual void OnHit()
        {

        }

        protected virtual void OnNotHit()
        {

        }

        public virtual void Init(BulletData data)
        {
            layerMask = GetLayerMask();
            delay = data.delay;
            leftDelay = 0;
            isHit = false;
            isDestroy = false;
            pierce = data.pierce;
            OnDamage = null;
        }

        //protected abstract void GetHitUnits();

        //protected void SubPierce()
        //{
        //    if (pierce-- == 1)
        //        bullet.Destroy();
        //}

        //protected bool CheckHitList(Unit unit)
        //{
        //    for (int i = 0; i < hitList.Count; i++)
        //        if (hitList[i].Equals(unit))
        //            return false;
        //    return true;
        //}

        //protected bool AddHitList(Unit unit)
        //{
        //    if (!unit && pierce >= 0)
        //    {
        //        bullet.Destroy();
        //        return false;
        //    }
        //    if(unit && CheckHitList(unit))
        //    {
        //        hitList.Add(unit);
        //        return true;
        //    }
        //    return false;
        //}

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

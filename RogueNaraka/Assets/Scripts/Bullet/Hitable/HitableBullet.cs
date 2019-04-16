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
        protected float leftDelay;


        protected bool isDestroy
        {
            get
            {
                return _isDestroy;
            }
            set
            {
                //Debug.Log(name + " Destroy " + value);
                _isDestroy = value;
            }
        }

        [SerializeField]
        protected bool _isDestroy;

        [SerializeField]
        protected bool isHitableWall;

        protected bool isHit;
        [SerializeField]
        protected bool isSplash;
        
        [SerializeField]
        protected int pierce
        {
            get
            {
                return _pierce;
            }
            set
            {
                //Debug.Log(name + " Set Pierce:" + value);
                _pierce = value;
            }
        }

        [SerializeField]
        protected int _pierce;

        public event System.Action<Bullet, Unit> OnDamage;

        private void Reset()
        {
            bullet = GetComponent<Bullet>();
            ownerable = GetComponent<OwnerableBullet>();
            shakeable = GetComponent<ShakeableBullet>();
        }

        //protected bool IsReady()
        //{
        //    if (leftDelay > 0)
        //    {
        //        leftDelay -= Time.deltaTime;
        //        return false;
        //    }
        //    else
        //        return true;
        //}

        //protected void HitFunc(bool value)
        //{
        //    if (value)
        //    {
        //        leftDelay = delay;

        //        OnHit();
        //        if (pierce-- == 1)
        //        {
        //            isDestroy = true;
        //            bullet.Destroy();
        //        }
        //        return;
        //    }
        //    else
        //    {
        //        OnNotHit();
        //    }
        //}

        IEnumerator DelayCorou()
        {
            //Debug.Log(name + ":DelayCorouStart");
            while (true)
            {
                if (leftDelay > 0)
                {
                    //yield return null;
                    //leftDelay -= Time.deltaTime;
                    yield return new WaitForFixedUpdate();
                    leftDelay -= Time.fixedDeltaTime;
                    //Debug.Log(name + ":DelayCorouDecrease");
                }
                else
                {
                    yield return null;
                    //Debug.Log(name + ":DelayCorouPass");
                }
            }
        }

        protected virtual void OnEnable()
        {
            StartCoroutine(DelayCorou());
        }

        //protected virtual void Update()
        //{
        //    if (isHit)
        //    {
        //        leftDelay = delay;
        //        isHit = false;

        //        OnHit();
        //        if (pierce-- == 1)
        //        {
        //            isDestroy = true;
        //            bullet.Destroy();
        //        }
        //        return;
        //    }
        //    else
        //    {
        //        OnNotHit();
        //    }
        //    //GetHitUnits();
        //    //for (int i = 0; i < hitList.Count; i++)
        //    //{
        //    //    //Debug.Log(name + " hit " + hitList[i].name);
                
        //    //    for(int j = 0; j < bullet.data.effects.Length; j++)
        //    //    {
        //    //        hitList[i].effectable.AddEffect(bullet.data.effects[j], bullet, ownerable.unit);
        //    //    }
        //    //    bullet.damageable.Damage(hitList[i], bullet.data.related);
        //    //    if(shakeable.shake.isOnHit)
        //    //        shakeable.Shake();
        //    //    if (OnDamage != null)
        //    //        OnDamage(bullet, hitList[i]);
        //    //}
        //    //hitList.Clear();
        //}

        public enum HIT
        {
            ENEMY, FRIENDLY, WALL, ETC
        }

        protected HIT Hit(Collider2D coll)
        {
            if (isDestroy || leftDelay > 0)
                return HIT.ETC;
            if ((layerMask.value & (1 << coll.gameObject.layer)) != (1 << coll.gameObject.layer))
                return HIT.FRIENDLY;

            Unit hit = coll.GetComponent<Unit>();

            if (hit)
            {
                //Debug.Log(name + "hit : " + hit.name + "leftDelay:" + leftDelay);
                for (int i = 0; i < bullet.data.effects.Length; i++)
                {
                    hit.effectable.AddEffect(bullet.data.effects[i], bullet, ownerable.unit);
                }

                bullet.damageable.Damage(hit, bullet.data.related);

                if (bullet.data.hitSFX.Length > 0)
                {
                    int rnd = Random.Range(0, bullet.data.hitSFX.Length);
                    string str = bullet.data.hitSFX[rnd];
                    AudioManager.instance.PlaySFX(str);
                }

                if (bullet.data.shake.isOnHit)
                    shakeable.Shake();

                if (OnDamage != null)
                    OnDamage(bullet, hit);
                return HIT.ENEMY;
            }
            else
            {
                
                if(bullet.data.isBounceable)
                {
                    //Debug.Log(name + " hit " + coll.name + ":wall");
                    BounceableBullet bounce = GetComponent<BounceableBullet>();
                    Vector3 pos = coll.transform.position;
                    BounceableBullet.DIRECTION direction;
                    if (pos.y > BoardManager.instance.boardRange[1].y)
                        direction = BounceableBullet.DIRECTION.UP;
                    else if (pos.x > BoardManager.instance.boardRange[1].x)
                        direction = BounceableBullet.DIRECTION.RIGHT;
                    else if (pos.y < BoardManager.instance.boardRange[0].y)
                        direction = BounceableBullet.DIRECTION.DOWN;
                    else
                        direction = BounceableBullet.DIRECTION.LEFT;

                    bounce.Bounce(direction);
                }
                return HIT.WALL;
            }
        }

        protected void CheckPierce()
        {
            leftDelay = delay;
            if(pierce <= 0 && !isDestroy)
            {
                isDestroy = true;
                bullet.Destroy();
            }
        }

        protected virtual void OnHit() { }

        protected virtual void OnNotHit() { }

        public virtual void Init(BulletData data)
        {
            layerMask = GetLayerMask();
            delay = data.delay;
            leftDelay = 0;
            isHit = false;
            isSplash = data.isSplash;
            isHitableWall = data.isHitableWall;
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

        public virtual void OnSpawn() { }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;
using RogueNaraka.BulletScripts.Hitable;

namespace RogueNaraka.BulletScripts
{
    public class Bullet : MonoBehaviour
    {
        #region field
        //[SerializeField]
        //HitableBullet _hitableRay;
        [SerializeField]
        HitableBulletCircleCast hitableCircleCast;
        [SerializeField]
        HitableBulletTrigger hitableTrigger;
        [SerializeField]
        HitableBulletRayCast hitableRaycast;

        public HitableBullet hitable;

        public ShootableBullet shootable;
        public MoveableBullet moveable;
        public OwnerableBullet ownerable;
        public TimeLimitableBullet timeLimitable;
        public DamageableBullet damageable;
        public SpawnableBullet spawnable;
        public ShakeableBullet shakeable;
        public DisapearableBullet disapearable;
        public GuideableBullet guideable;
        public SpinableBullet spinable;

        public Orderable orderable;

        public BulletData data;

        public Animator animator;

        public new SpriteRenderer renderer;

        public Rigidbody2D rigid;

        public PolygonCollider2D polygon;
        public CircleCollider2D circle;

        public PhysicsCollider physics;

        public Transform cachedTransform;

        IEnumerator deathCorou;

        #endregion

        void Reset()
        {
            animator = GetComponent<Animator>();
            renderer = GetComponent<SpriteRenderer>();
            rigid = GetComponent<Rigidbody2D>();

            //_hitable = GetComponent<HitableBullet>();
            //hitableRay = GetComponent<HitableBulletRay>();
            hitableCircleCast = GetComponent<HitableBulletCircleCast>();
            hitableTrigger = GetComponent<HitableBulletTrigger>();
            hitableRaycast = GetComponent<HitableBulletRayCast>();

            shootable = GetComponent<ShootableBullet>();
            moveable = GetComponent<MoveableBullet>();
            ownerable = GetComponent<OwnerableBullet>();
            timeLimitable = GetComponent<TimeLimitableBullet>();
            damageable = GetComponent<DamageableBullet>();
            spawnable = GetComponent<SpawnableBullet>();
            shakeable = GetComponent<ShakeableBullet>();
            disapearable = GetComponent<DisapearableBullet>();
            guideable = GetComponent<GuideableBullet>();
            orderable = GetComponent<Orderable>();
            spinable = GetComponent<SpinableBullet>();

            polygon = GetComponent<PolygonCollider2D>();
            circle = GetComponent<CircleCollider2D>();

            physics = GetComponent<PhysicsCollider>();
            cachedTransform = transform;
        }

        public void Init(Unit owner, BulletData data)
        {
            gameObject.SetActive(true);

            transform.rotation = Quaternion.identity;
            moveable.Init();
            moveable.enabled = false;

            ownerable.SetOwner(owner);
            this.data = (BulletData)data.Clone();
            name = this.data.name;

            if (owner.data.bulletColor == Color.clear)
                renderer.color = data.color;
            else
                renderer.color = owner.data.bulletColor;

            if (owner.data.bulletOrder != Order.Mid)
            {
                //Debug.Log(owner.name + "'s " + name + " sorting order is " + owner.data.bulletOrder);
                orderable.Init(owner.data.bulletOrder);
            }
            else
            {
                //Debug.Log(owner.name + "'s " + name + " sorting order is " + this.data.order);
                orderable.Init(this.data.order);
            }

            //Hitable
            if (hitable)
                hitable.enabled = false;

            switch (data.type)
            {
                case BULLET_TYPE.CIRCLE_CAST:
                    hitable = hitableCircleCast;
                    physics.enabled = false;
                    break;
                case BULLET_TYPE.DYNAMIC_CIRCLE:
                case BULLET_TYPE.DYNAMIC_POLY:
                    hitable = hitableTrigger;
                    physics.enabled = true;
                    break;
                case BULLET_TYPE.RAY_CAST:
                    hitable = hitableRaycast;
                    physics.enabled = false;
                    break;
                default:
                    break;
            }

            if (hitable)
            {
                hitable.Init(this.data);
                hitable.enabled = true;
            }

            animator.runtimeAnimatorController = this.data.controller;

            renderer.enabled = false;
            animator.enabled = false;

            deathCorou = null;

            timeLimitable.Init(this.data);
            timeLimitable.enabled = false;

            guideable.Init(this.data);
            guideable.enabled = false;

            disapearable.Init(this.data);

            spinable.Init(this.data);
        }

        public void Spawn(Unit owner, BulletData data, Vector3 position)
        {
            Init(owner, data);
            Spawn(position);
        }

        public void Spawn(Vector3 position)
        {
            if(hitable)
                hitable.enabled = true;
            moveable.enabled = true;
            if (data.limitTime != 0)
                timeLimitable.enabled = true;                
            base.GetComponent<Renderer>().enabled = true;
            animator.enabled = true;

            if (!data.shake.isOnHit && (data.shake.power != 0 || data.shake.time != 0))
                shakeable.Shake();
            if (data.spawnSFX.CompareTo(string.Empty) != 0)
                AudioManager.instance.PlaySFX(data.spawnSFX);

            transform.position = position;
            
            spawnable.Init(data);

            guideable.enabled = (guideable.rotateSpeed != 0);

            if (disapearable.duration != 0)
                disapearable.Disapear();

            spinable.enabled = data.spinSpeed != 0;

            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
            if (hitable)
                hitable.OnSpawn();
        }

        public void Destroy()
        {
            if (deathCorou == null)
            {
                deathCorou = DestroyCorou();
                StartCoroutine(deathCorou);
            }
        }

        public void DisableOnDestroy()
        {
            if(hitable)
                hitable.enabled = false;
            if (polygon.enabled)
                polygon.enabled = false;
            if (circle.enabled)
                circle.enabled = false;
            moveable.enabled = false;
        }

        IEnumerator DestroyCorou()
        {
            animator.SetBool("isDestroy", true);
            DisableOnDestroy();
            spawnable.OnDestroyBullet();
            AnimatorStateInfo state;
            do
            {
                yield return null;
                state = animator.GetCurrentAnimatorStateInfo(0);
            } while (state.normalizedTime < 1 || !state.IsName("Destroy"));
            BoardManager.instance.bulletPool.EnqueueObjectPool(gameObject, true);
        }
    }
}

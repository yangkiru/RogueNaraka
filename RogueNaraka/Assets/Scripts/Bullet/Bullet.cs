using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;
using RogueNaraka.BulletScripts.Hitable;
using DigitalRuby.AdvancedPolygonCollider;

namespace RogueNaraka.BulletScripts
{
    public class Bullet : MonoBehaviour
    {
        #region field
        //[SerializeField]
        //HitableBullet _hitableRay;
        [SerializeField]
        HitableBullet hitableCircleCast;
        [SerializeField]
        HitableBullet hitableTrigger;

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

        public AdvancedPolygonCollider advanced;

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
            advanced = GetComponent<AdvancedPolygonCollider>();

            cachedTransform = transform;
        }

        public void Init(Unit owner, BulletData data)
        {
            gameObject.SetActive(true);

            renderer.color = Color.white;

            transform.rotation = Quaternion.identity;
            moveable.Init();
            moveable.enabled = false;

            ownerable.SetOwner(owner);
            this.data = (BulletData)data.Clone();
            name = this.data.name;

            //Hitable
            if (hitable)
                hitable.enabled = false;

            switch (data.type)
            {
                case BULLET_TYPE.CIRCLE_CAST:
                    hitable = hitableCircleCast;
                    advanced.enabled = false;
                    break;
                case BULLET_TYPE.DYNAMIC_CIRCLE:
                case BULLET_TYPE.DYNAMIC_POLY:
                    hitable = hitableTrigger;
                    advanced.enabled = true;
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

            shakeable.Init(this.data.shake);

            guideable.Init(this.data);
            guideable.enabled = false;

            disapearable.Init(this.data);

            orderable.Init(this.data.order);

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

            if (shakeable.shake.power != 0 || shakeable.shake.time != 0)
                shakeable.Shake();
            if (data.spawnSFX.CompareTo(string.Empty) != 0)
                AudioManager.instance.PlaySFX(data.spawnSFX);

            transform.position = position;
            
            spawnable.Init(data);

            guideable.enabled = (guideable.rotateSpeed != 0);

            if (disapearable.duration != 0)
                disapearable.Disapear();

                spinable.enabled = data.spinSpeed != 0;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;
using RogueNaraka.BulletScripts.Hitable;

namespace RogueNaraka.BulletScripts
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField]
        HitableBullet hitableRay;
        [SerializeField]
        HitableBullet hitableCircle;
        [HideInInspector]
        public HitableBullet hitable;

        public ShootableBullet shootable;
        public MoveableBullet moveable;
        public OwnerableBullet ownerable;
        public TimeLimitableBullet timeLimitable;
        public DamageableBullet damageable;
        public SpawnableBullet spawnable;

        public BulletData data { get { return _data; } }
        BulletData _data;

        [SerializeField]
        Animator animator;

        [SerializeField]
        new SpriteRenderer renderer;

        IEnumerator deathCorou;

        void Reset()
        {
            hitableRay = GetComponent<HitableBulletRay>();
            hitableCircle = GetComponent<HitableBulletCircle>();
            shootable = GetComponent<ShootableBullet>();
            moveable = GetComponent<MoveableBullet>();
            ownerable = GetComponent<OwnerableBullet>();
            animator = GetComponent<Animator>();
            timeLimitable = GetComponent<TimeLimitableBullet>();
            damageable = GetComponent<DamageableBullet>();
            spawnable = GetComponent<SpawnableBullet>();
            renderer = GetComponent<SpriteRenderer>();
        }

        public void Init(Unit owner, BulletData data)
        {
            gameObject.SetActive(true);

            moveable.enabled = false;

            ownerable.SetOwner(owner);
            _data = (BulletData)data.Clone();
            name = _data.name;

            //Hitable
            DisableAllHitable();
            switch(_data.type)
            {
                case BULLET_TYPE.CIRCLECAST:
                    hitable = hitableCircle;
                    break;
                case BULLET_TYPE.RAYCAST:
                    hitable = hitableRay;
                    break;
            }
            
            hitable.Init(_data);

            animator.runtimeAnimatorController = data.controller;

            renderer.enabled = false;
            animator.enabled = false;

            deathCorou = null;

            timeLimitable.enabled = false;
        }

        void DisableAllHitable()
        {
            hitableRay.enabled = false;
            hitableCircle.enabled = false;
        }

        public void Spawn(Unit owner, BulletData data, Vector3 position)
        {
            Init(owner, data);
            Spawn(position);
        }

        public void Spawn(Vector3 position)
        {
            hitable.enabled = true;
            moveable.enabled = true;
            if (_data.limitTime != 0)
                timeLimitable.enabled = true;                
            renderer.enabled = true;
            animator.enabled = true;

            transform.position = position;
            
            spawnable.Init(_data);
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
            hitable.enabled = false;
            moveable.enabled = false;
        }

        IEnumerator DestroyCorou()
        {
            animator.SetBool("isDestroy", true);
            DisableOnDestroy();
            spawnable.OnDestroy();
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

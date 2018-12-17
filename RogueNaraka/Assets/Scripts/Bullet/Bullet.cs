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

        public BulletData data { get { return _data; } }
        BulletData _data;

        [SerializeField]
        Animator animator;

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
        }

        void Init(Unit owner, BulletData data)
        {
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
            hitable.enabled = true;
            hitable.Init(_data);

            moveable.enabled = true;
            animator.runtimeAnimatorController = data.controller;

            if (_data.limitTime != 0)
                timeLimitable.enabled = true;
            else
                timeLimitable.enabled = false;

            deathCorou = null;
        }

        void DisableAllHitable()
        {
            hitableRay.enabled = false;
            hitableCircle.enabled = false;
        }

        public void Spawn(Unit owner, BulletData data, Vector3 position)
        {
            Init(owner, data);
            transform.position = position;
            gameObject.SetActive(true);
        }

        public void Spawn(Vector3 position)
        {
            transform.position = position;
            gameObject.SetActive(true);
        }

        public void Destroy()
        {
            if (deathCorou == null)
            {
                deathCorou = DestroyCorou();
                StartCoroutine(deathCorou);
            }
        }

        public void DisableAll()
        {
            hitable.enabled = false;
            moveable.enabled = false;
        }

        IEnumerator DestroyCorou()
        {
            animator.SetBool("isDestroy", true);
            AnimatorStateInfo state;
            do
            {
                yield return null;
                state = animator.GetCurrentAnimatorStateInfo(0);
            } while (!state.IsName("Destroy"));
            
            DisableAll();
            
            do
            {
                yield return null;
            } while (state.normalizedTime < 1 && animator.IsInTransition(0));
            BoardManager.instance.bulletPool.EnqueueObjectPool(gameObject, true);
        }
    }
}

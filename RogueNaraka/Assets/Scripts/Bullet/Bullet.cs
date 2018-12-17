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

        public BulletData data { get { return _data; } }
        BulletData _data;

        [SerializeField]
        Animator animator;

        void Reset()
        {
            hitableRay = GetComponent<HitableBulletRay>();
            hitableCircle = GetComponent<HitableBulletCircle>();
            shootable = GetComponent<ShootableBullet>();
            moveable = GetComponent<MoveableBullet>();
            ownerable = GetComponent<OwnerableBullet>();
            animator = GetComponent<Animator>();
        }

        void Init(Unit owner, BulletData data)
        {
            ownerable.SetOwner(owner);
            _data = data;

            //Hitable
            DisableAllHitable();
            switch(data.type)
            {
                case BULLET_TYPE.CIRCLECAST:
                    hitable = hitableCircle;
                    break;
                case BULLET_TYPE.RAYCAST:
                    hitable = hitableRay;
                    break;
            }
            hitable.enabled = true;
            hitable.Init(data);

            moveable.enabled = true;
            animator.runtimeAnimatorController = data.controller;
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
            StartCoroutine(DestroyCorou());
        }

        public void DisableAll()
        {
            hitable.enabled = false;
            moveable.enabled = false;
        }

        IEnumerator DestroyCorou()
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            DisableAll();
            animator.SetBool("isDestroy", true);
            do
            {
                yield return null;
            } while (state.normalizedTime < 1);
            BoardManager.instance.bulletPool.EnqueueObjectPool(gameObject, true);
        }
    }
}

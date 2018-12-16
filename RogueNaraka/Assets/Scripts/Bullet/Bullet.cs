using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;
using RogueNaraka.BulletScripts.Hitable;

namespace RogueNaraka.BulletScripts
{
    public class Bullet : MonoBehaviour
    {
        HitableBullet hitableRay = null;
        HitableBullet hitableCircle = null;
        [HideInInspector]
        public HitableBullet hitable = null;

        [HideInInspector]
        public ShootableBullet shootable = null;
        [HideInInspector]
        public MoveableBullet moveable = null;
        [HideInInspector]
        public OwnerableBullet ownerable = null;

        public BulletData data { get { return _data; } }
        BulletData _data;

        Animator animator;

        void Awake()
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

        public void Destroy(bool isRemoveChild)
        {
            BoardManager.instance.bulletPool.EnqueueObjectPool(gameObject, isRemoveChild);
        }
    }
}

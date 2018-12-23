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

        public ShootableBullet shootable { get { return _shootable; } }
        [SerializeField]
        ShootableBullet _shootable;
        public MoveableBullet moveable { get { return _moveable; } }
        [SerializeField]
        MoveableBullet _moveable;
        public OwnerableBullet ownerable { get { return _ownerable; } }
        [SerializeField]
        OwnerableBullet _ownerable;
        public TimeLimitableBullet timeLimitable { get { return _timeLimitable; } }
        [SerializeField]
        TimeLimitableBullet _timeLimitable;
        public DamageableBullet damageable { get { return _damageable; } }
        [SerializeField]
        DamageableBullet _damageable;
        public SpawnableBullet spawnable { get { return _spawnable; } }
        [SerializeField]
        SpawnableBullet _spawnable;
        public ShakeableBullet shakeable { get { return _shakeable; } }
        [SerializeField]
        ShakeableBullet _shakeable;
        public DisapearableBullet disapearable { get { return _disapearable; } }
        [SerializeField]
        DisapearableBullet _disapearable;

        public BulletData data { get { return _data; } }
        [SerializeField]
        BulletData _data;

        public Animator animator { get { return _animator; } }
        [SerializeField]
        Animator _animator;

        public new SpriteRenderer renderer { get { return _renderer; } }
        [SerializeField]
        SpriteRenderer _renderer;

        IEnumerator deathCorou;

        void Reset()
        {
            _animator = GetComponent<Animator>();
            _renderer = GetComponent<SpriteRenderer>();

            hitableRay = GetComponent<HitableBulletRay>();
            hitableCircle = GetComponent<HitableBulletCircle>();

            _shootable = GetComponent<ShootableBullet>();
            _moveable = GetComponent<MoveableBullet>();
            _ownerable = GetComponent<OwnerableBullet>();
            _timeLimitable = GetComponent<TimeLimitableBullet>();
            _damageable = GetComponent<DamageableBullet>();
            _spawnable = GetComponent<SpawnableBullet>();
            _shakeable = GetComponent<ShakeableBullet>();
            _disapearable = GetComponent<DisapearableBullet>();
        }

        public void Init(Unit owner, BulletData data)
        {
            gameObject.SetActive(true);

            renderer.color = Color.white;

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

            timeLimitable.Init(data);
            timeLimitable.enabled = false;

            shakeable.Init(data.shake);
            shakeable.enabled = false;
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

            if (shakeable.shake.power != 0 || shakeable.shake.time != 0)
                shakeable.enabled = true;

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

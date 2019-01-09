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
        [SerializeField]
        HitableBullet hitableRay;
        [SerializeField]
        HitableBullet hitableCircle;

        public HitableBullet hitable { get { return _hitable; } }
        [SerializeField]
        HitableBullet _hitable;

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
        public GuideableBullet guideable { get { return _guideable; } }
        [SerializeField]
        GuideableBullet _guideable;

        public Orderable orderable { get { return _orderable; } }
        [SerializeField]
        Orderable _orderable;

        public BulletData data { get { return _data; } }
        [SerializeField]
        BulletData _data;

        public Animator animator { get { return _animator; } }
        [SerializeField]
        Animator _animator;

        public new SpriteRenderer renderer { get { return _renderer; } }
        [SerializeField]
        SpriteRenderer _renderer;

        public Rigidbody2D rigid { get { return _rigid; } }
        [SerializeField]
        Rigidbody2D _rigid;

        [SerializeField]
        PolygonCollider2D _polygon;
        [SerializeField]
        CircleCollider2D _circle;

        [SerializeField]
        AdvancedPolygonCollider _advanced;

        IEnumerator deathCorou;

        #endregion

        void Reset()
        {
            _animator = GetComponent<Animator>();
            _renderer = GetComponent<SpriteRenderer>();
            _rigid = GetComponent<Rigidbody2D>();

            _hitable = GetComponent<HitableBullet>();
            //hitableRay = GetComponent<HitableBulletRay>();
            //hitableCircle = GetComponent<HitableBulletCircle>();

            _shootable = GetComponent<ShootableBullet>();
            _moveable = GetComponent<MoveableBullet>();
            _ownerable = GetComponent<OwnerableBullet>();
            _timeLimitable = GetComponent<TimeLimitableBullet>();
            _damageable = GetComponent<DamageableBullet>();
            _spawnable = GetComponent<SpawnableBullet>();
            _shakeable = GetComponent<ShakeableBullet>();
            _disapearable = GetComponent<DisapearableBullet>();
            _guideable = GetComponent<GuideableBullet>();
            _orderable = GetComponent<Orderable>();

            _polygon = GetComponent<PolygonCollider2D>();
            _circle = GetComponent<CircleCollider2D>();
            _advanced = GetComponent<AdvancedPolygonCollider>();
        }

        public void Init(Unit owner, BulletData data)
        {
            gameObject.SetActive(true);

            _renderer.color = Color.white;

            transform.rotation = Quaternion.identity;
            _moveable.Init();
            _moveable.enabled = false;

            _ownerable.SetOwner(owner);
            _data = (BulletData)data.Clone();
            name = _data.name;

            //Hitable
            //DisableAllHitable();
            switch (_data.type)
            {
                case BULLET_TYPE.CIRCLE:
                    _circle.enabled = true;
                    _polygon.enabled = false;
                    _advanced.enabled = false;
                    _circle.radius = _data.size;
                    break;
                case BULLET_TYPE.DYNAMIC:
                    _circle.enabled = false;
                    _polygon.enabled = true;
                    _advanced.enabled = true;
                    break;
                default:
                    break;
            }

            _hitable.Init(_data);

            _animator.runtimeAnimatorController = _data.controller;

            _renderer.enabled = false;
            _animator.enabled = false;

            deathCorou = null;

            _timeLimitable.Init(_data);
            _timeLimitable.enabled = false;

            _shakeable.Init(_data.shake);

            _guideable.Init(_data);
            _guideable.enabled = false;

            _disapearable.Init(_data);

            _orderable.Init(_data.order);
        }

        void DisableAllHitable()
        {
            //hitableRay.enabled = false;
            //hitableCircle.enabled = false;
        }

        public void Spawn(Unit owner, BulletData data, Vector3 position)
        {
            Init(owner, data);
            Spawn(position);
        }

        public void Spawn(Vector3 position)
        {
            if(_hitable)
                _hitable.enabled = true;
            moveable.enabled = true;
            if (_data.limitTime != 0)
                timeLimitable.enabled = true;                
            renderer.enabled = true;
            animator.enabled = true;

            if (shakeable.shake.power != 0 || shakeable.shake.time != 0)
                shakeable.Shake();

            transform.position = position;
            
            spawnable.Init(_data);

            if (guideable.rotateSpeed != 0)
                guideable.enabled = true;

            if (disapearable.duration != 0)
                disapearable.Disapear();
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
            if(_hitable)
                _hitable.enabled = false;
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

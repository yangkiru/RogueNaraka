using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts.Targetable;
using RogueNaraka.UnitScripts.AutoMoveable;
using RogueNaraka.UnitScripts.Attackable;

namespace RogueNaraka.UnitScripts
{
    public class Unit : MonoBehaviour
    {
        #region field
        public MoveableUnit moveable { get { return _moveable; } }
        [SerializeField]
        MoveableUnit _moveable;

        #region attackable

        public AttackableUnit attackable { get { return _attackable; } }
        AttackableUnit _attackable;
        [SerializeField]
        StopBeforeAttackableUnit _stopBeforeAttackable;
        [SerializeField]
        StopAfterAttackableUnit _stopAfterAttackable;
        [SerializeField]
        DontStopAttackableUnit _dontStopAttackable;

        #endregion

        #region targetable

        public TargetableUnit targetable { get { return _targetable; } }
        TargetableUnit _targetable;
        [SerializeField]
        EnemyTargetableUnit _enemyTargetable;
        [SerializeField]
        FriendlyTargetableUnit _friendlyTargetable;

        #endregion

        #region autoMoveable

        public AutoMoveableUnit autoMoveable { get { return _autoMoveable; } }
        [SerializeField]
        AutoMoveableUnit _autoMoveable;
        [SerializeField]
        RandomMoveableUnit _randomMoveable;
        [SerializeField]
        RushMoveableUnit _rushMoveable;
        [SerializeField]
        RestRushMoveableUnit _restRushMoveable;
        public FollowMoveableUnit followMoveable { get { return _followMoveable; } }
        [SerializeField]
        FollowMoveableUnit _followMoveable;
        [SerializeField]
        Boss0MoveableUnit _boss0Moveable;

        #endregion

        #region etc-able

        public DamageableUnit damageable { get { return _damageable; } }
        [SerializeField]
        DamageableUnit _damageable;

        public HpableUnit hpable { get { return _hpable; } }
        [SerializeField]
        HpableUnit _hpable;
        public MpableUnit mpable { get { return _mpable; } }
        [SerializeField]
        MpableUnit _mpable;

        public DeathableUnit deathable { get { return _deathable; } }
        [SerializeField]
        DeathableUnit _deathable;

        public EffectableUnit effectable { get { return _effectable; } }
        [SerializeField]
        EffectableUnit _effectable;

        public TimeLimitableUnit timeLimitable { get { return _timeLimitable; } }
        [SerializeField]
        TimeLimitableUnit _timeLimitable;

        public Orderable orderable { get { return _orderable; } }
        [SerializeField]
        Orderable _orderable;

        public FollowableUnit followable { get { return _followable; } }
        [SerializeField]
        FollowableUnit _followable;

        #endregion

        #region etc

        public TackleableUnit tackleable { get { return _tackleable; } }
        [SerializeField]
        TackleableUnit _tackleable;

        public Animator animator { get { return _animator; } }
        [SerializeField]
        Animator _animator;

        public UnitData data { get { return _data; } }
        [SerializeField]
        UnitData _data;

        public Rigidbody2D rigid { get { return _rigid; } }
        [SerializeField]
        Rigidbody2D _rigid;

        public new SpriteRenderer renderer { get { return _renderer; } }
        [SerializeField]
        SpriteRenderer _renderer;

        public new Collider2D collider { get { return _collider; } }
        [SerializeField]
        Collider2D _collider;

        public Transform cachedTransform;

        public Stat stat { get { return _data.stat; } }

        public bool isStun;

        #endregion

        #endregion

        void Reset()
        {
            _moveable = GetComponent<MoveableUnit>();
            _stopBeforeAttackable = GetComponent<StopBeforeAttackableUnit>();
            _stopAfterAttackable = GetComponent<StopAfterAttackableUnit>();
            _dontStopAttackable = GetComponent<DontStopAttackableUnit>();

            _enemyTargetable = GetComponent<EnemyTargetableUnit>();
            _friendlyTargetable = GetComponent<FriendlyTargetableUnit>();
            _randomMoveable = GetComponent<RandomMoveableUnit>();
            _rushMoveable = GetComponent<RushMoveableUnit>();
            _restRushMoveable = GetComponent<RestRushMoveableUnit>();
            _followMoveable = GetComponent<FollowMoveableUnit>();
            _boss0Moveable = GetComponent<Boss0MoveableUnit>();

            _damageable = GetComponent<DamageableUnit>();
            _hpable = GetComponent<HpableUnit>();
            _mpable = GetComponent<MpableUnit>();
            _deathable = GetComponent<DeathableUnit>();
            _effectable = GetComponent<EffectableUnit>();
            _timeLimitable = GetComponent<TimeLimitableUnit>();
            _orderable = GetComponent<Orderable>();
            _followable = GetComponent<FollowableUnit>();

            _animator = GetComponent<Animator>();
            _rigid = GetComponent<Rigidbody2D>();
            _renderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();
        }

        public void OnDisable()
        {
            if (!Application.isPlaying)
                return;
            if (_data.isFriendly)
            {
                if(BoardManager.instance.friendlies.Remove(this))
                    Debug.Log("Friendly Remove:" + name);
                else
                    Debug.Log("Friendly Remove Fail:" + name);
            }
            else
                BoardManager.instance.enemies.Remove(this);
        }

        public void SetStat(Stat stat)
        {
            _data.stat = (Stat)stat.Clone();
        }

        public void Init(UnitData data)
        {
            //Debug.Log(data.name + " Init");
            _data = (UnitData)data.Clone();
            name = _data.name;
            if (_data.isFriendly)
                gameObject.layer = GameDatabase.friendlyLayer;
            else
                gameObject.layer = GameDatabase.enemyLayer;

            deathable.Init();
            _animator.runtimeAnimatorController = _data.controller;

            _moveable.Init(_data);
            _moveable.enabled = true;

            if (_attackable)
                _attackable.enabled = false;
            if (_data.weapon >= 0)
            {
                switch (GameDatabase.instance.weapons[_data.weapon].type)
                {
                    case ATTACK_TYPE.STOP_BEFORE:
                        _attackable = _stopBeforeAttackable;
                        break;
                    case ATTACK_TYPE.STOP_AFTER:
                        _attackable = _stopAfterAttackable;
                        break;
                    case ATTACK_TYPE.DONT_STOP:
                        _attackable = _dontStopAttackable;
                        break;
                }
                _attackable.Init(_data);
                _attackable.enabled = true;
            }

            DisableTargetables();
            if (_data.isFriendly)
                _targetable = _enemyTargetable;
            else
                _targetable = _friendlyTargetable;
            _targetable.enabled = true;

            DisableAutoMoveables();
            switch(_data.move)
            {
                case MOVE_TYPE.RANDOM:
                    _autoMoveable = _randomMoveable;
                    break;
                case MOVE_TYPE.RUSH:
                    _autoMoveable = _rushMoveable;
                    break;
                case MOVE_TYPE.REST_RUSH:
                    _autoMoveable = _restRushMoveable;
                    break;
                case MOVE_TYPE.FOLLOW:
                    _autoMoveable = _followMoveable;
                    break;
                case MOVE_TYPE.BOSS0:
                    _autoMoveable = _boss0Moveable;
                    break;
                default:
                    _autoMoveable = null;
                    break;
            }
            if (_autoMoveable)
            {
                _autoMoveable.Init(_data);
                _autoMoveable.enabled = true;
            }

            _hpable.Init(_data.stat);
            _mpable.Init(_data.stat);
            _hpable.enabled = true;
            _mpable.enabled = true;

            _timeLimitable.enabled = false;
            _timeLimitable.Init(_data.limitTime);

            _followable.Init(_data);

            _orderable.Init(data.order);


            if (_data.effects != null)
            {
                for (int i = 0; i < _data.effects.Length; i++)
                    _effectable.AddEffect(_data.effects[i]);
            }

            _renderer.color = _data.color;
            _collider.isTrigger = false;
            _collider.enabled = true;

            _tackleable.Init(data);
        }

        public void Spawn(Vector3 position)
        {
            cachedTransform.position = position;

            gameObject.SetActive(true);
            if (_data.isFriendly && !BoardManager.instance.friendlies.Contains(this))
            {
                Debug.Log("Friendly:" + name);
                BoardManager.instance.friendlies.Add(this);
            }
            else if (!_data.isFriendly && !BoardManager.instance.enemies.Contains(this))
            {
                Debug.Log("Enemy:" + name);
                BoardManager.instance.enemies.Add(this);
            }
            if (_timeLimitable.time != 0)
                _timeLimitable.enabled = true;
            if (_hpable.currentHp <= 0)
                _deathable.Death();
        }

        public void Teleport(Vector3 position)
        {
            cachedTransform.position = position;
        }

        public void PlaySFX(string name)
        {
            AudioManager.instance.PlaySFX(name);
        }

        void DisableAutoMoveables()
        {
            _randomMoveable.enabled = false;
            _rushMoveable.enabled = false;
            _followMoveable.enabled = false;
        }

        void DisableTargetables()
        {
            _enemyTargetable.enabled = false;
            _friendlyTargetable.enabled = false;
        }

        public void DisableAll()
        {
            hpable.enabled = false;
            mpable.enabled = false;
            moveable.enabled = false;
            if(autoMoveable) autoMoveable.enabled = false;
            if(attackable) attackable.enabled = false;
            targetable.enabled = false;
            moveable.agent.enabled = false;
        }

        public void Kill(bool isTxt = true)
        {
            if (isTxt)
                damageable.Damage(hpable.currentHp);
            else
                hpable.AddHp(-stat.hp);
        }
    }
}
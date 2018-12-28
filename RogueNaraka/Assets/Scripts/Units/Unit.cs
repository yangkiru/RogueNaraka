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

        public AttackableUnit attackable { get { return _attackable; } }
        AttackableUnit _attackable;
        [SerializeField]
        StopBeforeAttackableUnit _stopBeforeAttackable;
        [SerializeField]
        StopAfterAttackableUnit _stopAfterAttackable;
        [SerializeField]
        DontStopAttackableUnit _dontStopAttackable;

        public TargetableUnit targetable { get { return _targetable; } }
        TargetableUnit _targetable;
        [SerializeField]
        EnemyTargetableUnit _enemyTargetable;
        [SerializeField]
        FriendlyTargetableUnit _friendlyTargetable;

        public AutoMoveableUnit autoMoveable { get { return _autoMoveable; } }
        AutoMoveableUnit _autoMoveable;
        [SerializeField]
        RandomMoveableUnit _randomMoveable;
        [SerializeField]
        RushMoveableUnit _rushMoveable;
        [SerializeField]
        RestRushMoveableUnit _restRushMoveable;

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

        public Animator animator { get { return _animator; } }
        [SerializeField]
        Animator _animator;

        public UnitData data { get { return _data; } }
        [SerializeField]
        UnitData _data;

        public Stat stat { get { return _data.stat; } }

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
            _damageable = GetComponent<DamageableUnit>();
            _hpable = GetComponent<HpableUnit>();
            _mpable = GetComponent<MpableUnit>();
            _deathable = GetComponent<DeathableUnit>();
            _effectable = GetComponent<EffectableUnit>();
            _timeLimitable = GetComponent<TimeLimitableUnit>();

            _orderable = GetComponent<Orderable>();

            _animator = GetComponent<Animator>();
        }

        void OnDisable()
        {
            if (!Application.isPlaying)
                return;
            if(_data.isFriendly)
                BoardManager.instance.friendlies.Remove(this);
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

            DisableAttackables();
            switch(GameDatabase.instance.weapons[_data.weapon].type)
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
            _timeLimitable.Init(data.limitTime);

            _orderable.Init(data.order);

            if(_data.effects != null)
                for (int i = 0; i < _data.effects.Length; i++)
                    _effectable.AddEffect(_data.effects[i]);
        }

        public void Spawn(Vector3 position)
        {
            transform.position = position;

            gameObject.SetActive(true);
            if (_data.isFriendly && !BoardManager.instance.friendlies.Contains(this))
                BoardManager.instance.friendlies.Add(this);
            else if (!_data.isFriendly && !BoardManager.instance.enemies.Contains(this))
                BoardManager.instance.enemies.Add(this);
            if (_timeLimitable.time != 0)
                _timeLimitable.enabled = true;
        }

        void DisableAutoMoveables()
        {
            _randomMoveable.enabled = false;
            _rushMoveable.enabled = false;
        }

        void DisableTargetables()
        {
            _enemyTargetable.enabled = false;
            _friendlyTargetable.enabled = false;
        }
    
        void DisableAttackables()
        {
            _stopAfterAttackable.enabled = false;
            _stopBeforeAttackable.enabled = false;
        }

        public void DisableAll()
        {
            hpable.enabled = false;
            mpable.enabled = false;
            moveable.enabled = false;
            if(autoMoveable) autoMoveable.enabled = false;
            attackable.enabled = false;
            targetable.enabled = false;
            moveable.agent.enabled = false;
        }

        public void Kill(bool isTxt = true)
        {
            if (isTxt)
                damageable.Damage(-hpable.currentHp);
            else
                hpable.AddHp(-hpable.currentHp);
        }
    }
}
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
        public MoveableUnit moveable { get { return _moveable; } }
        [SerializeField]
        protected MoveableUnit _moveable;

        public AttackableUnit attackable { get { return _attackable; } }
        protected AttackableUnit _attackable;
        [SerializeField]
        protected StopBeforeAttackableUnit _stopBeforeAttackable;
        [SerializeField]
        protected StopAfterAttackableUnit _stopAfterAttackable;
        [SerializeField]
        protected DontStopAttackableUnit _dontStopAttackable;

        public TargetableUnit targetable { get { return _targetable; } }
        protected TargetableUnit _targetable;
        [SerializeField]
        protected EnemyTargetableUnit _enemyTargetable;
        [SerializeField]
        protected FriendlyTargetableUnit _friendlyTargetable;

        public AutoMoveableUnit autoMoveable { get { return _autoMoveable; } }
        protected AutoMoveableUnit _autoMoveable;
        [SerializeField]
        protected RandomMoveableUnit _randomMoveable;
        [SerializeField]
        protected RushMoveableUnit _rushMoveable;

        public DamageableUnit damageable { get { return _damageable; } }
        [SerializeField]
        protected DamageableUnit _damageable;

        public HpableUnit hpable { get { return _hpable; } }
        [SerializeField]
        protected HpableUnit _hpable;
        public MpableUnit mpable { get { return _mpable; } }
        [SerializeField]
        protected MpableUnit _mpable;

        public DeathableUnit deathable { get { return _deathable; } }
        [SerializeField]
        protected DeathableUnit _deathable;

        public EffectableUnit effectable { get { return _effectable; } }
        [SerializeField]
        protected EffectableUnit _effectable;

        public Animator animator { get { return _animator; } }
        [SerializeField]
        Animator _animator;

        public UnitData data { get { return _data; } }
        [SerializeField]
        UnitData _data;

        void OnDisable()
        {
            BoardManager.instance.friendlies.Remove(this);
            BoardManager.instance.enemies.Remove(this);
        }

        public void SetStat(Stat stat)
        {
            _data.stat = (Stat)stat.Clone();
        }

        public void Init(UnitData data)
        {
            Debug.Log(data.name + " Init");
            _data = (UnitData)data.Clone();
            name = _data.name;
            if (_data.isFriendly)
                gameObject.layer = GameDatabase.friendlyLayer;
            else
                gameObject.layer = GameDatabase.enemyLayer;

            deathable.Init();
            _animator.runtimeAnimatorController = _data.controller;

            _moveable.Init(_data);

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
            }
            _autoMoveable.Init(_data);
            _autoMoveable.enabled = true;

            _hpable.Init(_data.stat);
            _mpable.Init(_data.stat);
            _hpable.enabled = true;
            _mpable.enabled = true;
        }

        public void Spawn(Vector3 position)
        {
            transform.position = position;

            gameObject.SetActive(true);
            if (_data.isFriendly)
                BoardManager.instance.friendlies.Add(this);
            else
                BoardManager.instance.enemies.Add(this);
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
            _damageable = GetComponent<DamageableUnit>();
            _hpable = GetComponent<HpableUnit>();
            _mpable = GetComponent<MpableUnit>();
            _deathable = GetComponent<DeathableUnit>();
            _effectable = GetComponent<EffectableUnit>();

            _animator = GetComponent<Animator>();
        }
    }
}
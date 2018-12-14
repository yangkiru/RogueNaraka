using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts.Targetable;
using RogueNaraka.UnitScripts.AutoMoveable;

namespace RogueNaraka.UnitScripts
{
    public class Unit : MonoBehaviour
    {
        public MoveableUnit moveable { get { return _moveable; } }
        protected MoveableUnit _moveable;

        public AttackableUnit attackable { get { return _attackable; } }
        protected AttackableUnit _attackable;

        public TargetableUnit targetable { get { return _targetable; } }
        protected TargetableUnit _targetable;
        protected EnemyTargetableUnit _enemyTargetable;
        protected FriendlyTargetableUnit _friendlyTargetable;

        public AutoMoveableUnit autoMoveable { get { return _autoMoveable; } }
        protected AutoMoveableUnit _autoMoveable;
        protected RandomMoveableUnit _randomMoveable;
        protected RushMoveableUnit _rushMoveable;

        public DamageableUnit damageable { get { return _damageable; } }
        protected DamageableUnit _damageable;

        public HpableUnit hpable { get { return _hpable; } }
        protected HpableUnit _hpable;
        public MpableUnit mpable { get { return _mpable; } }
        protected MpableUnit _mpable;

        public DeathableUnit deathable { get { return _deathable; } }
        protected DeathableUnit _deathable;

        public EffectableUnit effectable { get { return _effectable; } }
        protected EffectableUnit _effectable;

        public Animator animator { get { return _animator; } }
        Animator _animator;

        public UnitData data { get { return _data; } }
        UnitData _data;

        void Awake()
        {
            _moveable = GetComponent<MoveableUnit>();
            _attackable = GetComponent<AttackableUnit>();
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

        public void SetStat(Stat stat)
        {
            _data.stat = (Stat)stat.Clone();
        }

        public void Init(UnitData data)
        {
            _data = (UnitData)data.Clone();
            deathable.Init();
            _animator.runtimeAnimatorController = data.controller;

            _moveable.Init(data);
            _attackable.Init(data);

            DisableTargetables();
            if (data.isFriendly)
                _targetable = _enemyTargetable;
            else
                _targetable = _friendlyTargetable;
            _targetable.enabled = true;

            DisableAutoMoveables();
            switch(data.move)
            {
                case MOVE_TYPE.RANDOM:
                    _autoMoveable = _randomMoveable;
                    break;
                case MOVE_TYPE.RUSH:
                    _autoMoveable = _rushMoveable;
                    break;
            }
            _autoMoveable.Init(data);
            _autoMoveable.enabled = true;

            _hpable.Init(data.stat);
            _mpable.Init(data.stat);
        }

        public void Spawn(Vector3 position)
        {
            transform.position = position;
            gameObject.SetActive(true);
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
    }
}
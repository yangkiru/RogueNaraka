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

        public DamageableUnit damageable { get { return _damageable; } }
        protected DamageableUnit _damageable;
        public HpableUnit hpable { get { return _hpable; } }
        protected HpableUnit _hpable;
        public MpableUnit mpable { get { return _mpable; } }
        protected MpableUnit _mpable;

        void Awake()
        {
            _moveable = GetComponent<MoveableUnit>();
            _attackable = GetComponent<AttackableUnit>();
            _enemyTargetable = GetComponent<EnemyTargetableUnit>();
            _friendlyTargetable = GetComponent<FriendlyTargetableUnit>();
            _randomMoveable = GetComponent<RandomMoveableUnit>();
            _damageable = GetComponent<DamageableUnit>();
            _hpable = GetComponent<HpableUnit>();
            _mpable = GetComponent<MpableUnit>();
        }

        public void Init(UnitData data)
        {
            _moveable.Init(data);
            _attackable.Init(data);

            if (data.isFriendly)
                _targetable = _enemyTargetable;
            else
                _targetable = _friendlyTargetable;
            switch(data.move)
            {
                case MOVE_TYPE.RANDOM:
                    _autoMoveable = _randomMoveable;
                    break;
                case MOVE_TYPE.RUSH:
                    _autoMoveable = 
            }
        }
    }
}
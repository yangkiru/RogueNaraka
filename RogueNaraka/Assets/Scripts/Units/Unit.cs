using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts.Targetable;
using RogueNaraka.UnitScripts.AutoMoveableUnit;

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
        public RandomMoveableUnit randomMoveable { get { return _randomMoveable; } }
        protected RandomMoveableUnit _randomMoveable;

        void Awake()
        {
            _moveable = GetComponent<MoveableUnit>();
            _attackable = GetComponent<AttackableUnit>();
            _targetable = GetComponent<TargetableUnit>();
            _randomMoveable = GetComponent<RandomMoveableUnit>();
        }
    }
}
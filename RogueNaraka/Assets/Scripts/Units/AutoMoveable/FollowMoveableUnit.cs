using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts.AutoMoveable
{
    /// <summary>
    /// 실제로 이용하기 위해서는 FollowableUnit.AddFollower를 이용
    /// </summary>
    public class FollowMoveableUnit : AutoMoveableUnit
    {
        public Unit target { get { return _target; } }
        [SerializeField]
        Unit _target;
        [SerializeField]
        Unit _following;

        public void SetTarget(Unit unit)
        {
            _target = unit;
        }

        public void LostTarget()
        {
            _target = null;
            _following = null;
        }

        protected override void AutoMove()
        {
            if (_target && _target.followable.followers.Count > 1)
            {
                _following = _target.followable.followers[target.followable.followers.Count - 1];
            }
            else if (_target)
            {
                _following = _target;
            }
            else
                _following = null;
        }

        private void LateUpdate()
        {
            if (_following)
            {
                moveable.Move(target.transform.position);
            }
        }

        private void OnDisable()
        {
            if (_target)
            {
                if (_target.gameObject.activeSelf)
                    _target.followable.RemoveFollower(unit);
                LostTarget();
            }
        }
    }
}
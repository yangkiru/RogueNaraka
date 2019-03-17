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
            Debug.Log("SetTarget" + this.unit.name + " " + unit.name);
            _target = unit;
        }

        public void LostTarget()
        {
            Debug.Log("LostTarget" + this.unit.name + " " + unit.name);
            _target = null;
            _following = null;
        }

        protected override void AutoMove()
        {
            if (_target)
            {
                int index = target.followable.followers.FindIndex(0, x => x.GetInstanceID() == unit.GetInstanceID());
                if(index > 0)
                    _following = _target.followable.followers[index - 1];
                else
                    _following = _target;
            }
            else
                _following = null;
        }

        private void LateUpdate()
        {
            if (_following)
            {
                moveable.Move(_following.transform.position);
            }
        }

        public void OnDeath()
        {
            if (_target)
            {
                if (_target.gameObject.activeSelf)
                    _target.followable.RemoveFollower(unit);
                //LostTarget();
            }
        }

        private void OnDisable()
        {
            OnDeath();
        }
    }
}
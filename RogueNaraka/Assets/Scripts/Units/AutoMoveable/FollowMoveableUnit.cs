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

                if(currentCorou == null)
                {
                    currentCorou = Follow();
                    StartCoroutine(currentCorou);
                }
            }
            else
                _following = null;
        }

        IEnumerator currentCorou;
        
        IEnumerator Follow()
        {
            Queue<Vector2> queue = new Queue<Vector2>(60);
            unit.cashedTransform.position = _following.cashedTransform.position;
            float t = 0.3f;
            do
            {
                yield return null;
                if (!_following)
                {
                    currentCorou = null;
                    yield break;
                }
                queue.Enqueue(_following.cashedTransform.position);
                if (t > 0)
                {
                    t -= Time.deltaTime;
                    unit.cashedTransform.position = queue.Peek();
                }
                else
                {
                    unit.cashedTransform.position = queue.Dequeue();
                }
            } while (!unit.deathable.isDeath || _following == null);
            currentCorou = null;
        }

        public void OnDeath()
        {
            if (_target)
            {
                if (_target.gameObject.activeSelf)
                    _target.followable.RemoveFollower(unit);
                //LostTarget();
            }
            currentCorou = null;
        }

        private void OnDisable()
        {
            OnDeath();
        }
    }
}
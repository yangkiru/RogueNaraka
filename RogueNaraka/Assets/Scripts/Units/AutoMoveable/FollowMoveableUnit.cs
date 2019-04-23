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
        Unit target;
        public LinkedListNode<Unit> Node { get { return node; } }
        LinkedListNode<Unit> node;

        public void Init(LinkedListNode<Unit> node, Unit target)
        {
            Debug.Log("Init" + this.unit.name);
            this.node = node;
            this.target = target;
        }

        public void OnDisable()
        {
            if(target != null)
                target.followable.RemoveFollower(this.unit);
            this.node = null;
            this.target = null;
        }

        public void OnLostTarget()
        {
            this.node = null;
            this.target = null;
        }

        protected override void AutoMove()
        {
            if (this.target && !this.target.followable.isFollow)
                return;
            if(this.node != null)
            {
                if(this.node.Previous == null)
                {
                    moveable.SetDestination(target.cachedTransform.position);
                }
                else
                {
                    moveable.SetDestination(node.Previous.Value.cachedTransform.position);
                }

            }
        }
    }
}
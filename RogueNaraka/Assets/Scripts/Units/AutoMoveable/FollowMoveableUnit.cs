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
                
                //if (currentCorou == null)
                //{
                //    currentCorou = FollowCorou();
                //    StartCoroutine(currentCorou);
                //}
            }
            //if (target)
            //{
            //    int index = target.followable.Followers.FindIndex(0, x => x.GetInstanceID() == unit.GetInstanceID());
            //    if(index > 0)
            //        _following = target.followable.Followers[index - 1];
            //    else
            //        _following = target;

            //    if(currentCorou == null)
            //    {
            //        currentCorou = Follow();
            //        StartCoroutine(currentCorou);
            //    }
            //}
            //else
            //    _following = null;
        }

        //IEnumerator currentCorou;
        
        //IEnumerator FollowCorou()
        //{
        //    Queue<Vector2> queue = new Queue<Vector2>(30);
        //    unit.cachedTransform.position = _following.cachedTransform.position;
        //    float t = 0.3f;
        //    do
        //    {
        //        yield return null;
        //        queue.Enqueue(_following.cachedTransform.position);
        //        if (t > 0)
        //        {
        //            t -= Time.deltaTime;
        //            unit.cachedTransform.position = queue.Peek();
        //        }
        //        else
        //        {
        //            unit.cachedTransform.position = queue.Dequeue();
        //        }
        //    } while (!unit.deathable.isDeath || _following == null);
        //    currentCorou = null;
        //}

        //public void OnDeath()
        //{
        //    if (target)
        //    {
        //        if (target.gameObject.activeSelf)
        //            target.followable.RemoveFollower(unit);
        //        //LostTarget();
        //    }
        //    if (currentCorou != null)
        //        StopCoroutine(currentCorou);
        //    currentCorou = null;
        //}
    }
}
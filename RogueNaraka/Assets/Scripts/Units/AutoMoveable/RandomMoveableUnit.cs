using RogueNaraka.UnitScripts.Targetable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts.AutoMoveable
{
    public class RandomMoveableUnit : AutoMoveableUnit
    {
        TargetableUnit targetable;
        public override void Init(UnitData data)
        {
            base.Init(data);
            targetable = unit.targetable;
        }


        protected override void AutoMove()
        {
            if (targetable && targetable.target && unit.targetable.targetDistance > unit.attackable.weapon.attackDistance)
            {
                Vector2 vec = targetable.target.transform.position - transform.position;
                Vector2 goal = (Vector2)cashedTransform.position + vec.normalized * Mathf.Min(distance, targetable.targetDistance);
                moveable.Move(goal);
                //Debug.DrawLine(cashedTransform.position, goal, Color.white, 2);
            }
            else
            {
                Vector2 rnd = Random.insideUnitCircle.normalized * distance;
                Vector2 goal = (Vector2)cashedTransform.position + rnd;
                //Debug.DrawLine(cashedTransform.position, goal, Color.white, 2);
                moveable.Move(goal);
            }
        }
    }
}
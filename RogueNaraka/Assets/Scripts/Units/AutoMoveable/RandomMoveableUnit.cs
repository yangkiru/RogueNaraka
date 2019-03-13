using RogueNaraka.UnitScripts.Targetable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts.AutoMoveable
{
    public class RandomMoveableUnit : AutoMoveableUnit
    {
        public override void Init(UnitData data)
        {
            base.Init(data);
        }


        protected override void AutoMove()
        {
            if (unit.targetable && unit.targetable.target && unit.targetable.targetDistance > unit.attackable.weapon.attackDistance)
            {
                Vector2 vec = unit.targetable.target.cashedTransform.position - cashedTransform.position;
                float s = unit.attackable.weapon.attackDistance;
                float tss = unit.targetable.targetDistance - s;
                float m = unit.data.moveDistance;

                float distance = (tss > m) ? m : unit.targetable.targetDistance;
                Vector2 goal = (Vector2)cashedTransform.position + vec.normalized * distance;
                moveable.Move(goal);
                leftDelay = unit.data.moveDelay * 0.5f;
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
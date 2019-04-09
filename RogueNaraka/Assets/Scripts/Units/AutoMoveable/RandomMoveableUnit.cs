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
                Vector2 vec = unit.targetable.target.cachedTransform.position - unit.cachedTransform.position;
                float unitToAttackDistance = unit.targetable.targetDistance - unit.attackable.weapon.attackDistance;

                Vector2 goal = (Vector2)unit.cachedTransform.position + vec.normalized * Mathf.Max(Mathf.Min(distance, unitToAttackDistance), 0.5f);
                //if (!BoardManager.IsPointInBoard(goal))
                Debug.DrawLine(unit.cachedTransform.position, goal, Color.yellow, 1);
                moveable.SetDestination(goal);

                //Debug.DrawLine(cashedTransform.position, goal, Color.white, 2);
            }
            else if(unit.targetable && unit.targetable.target)
            {
                Vector2 goal;
                float oppositeValue = 0.75f;
                do
                {
                    Vector2 oppositeVector = (unit.cachedTransform.position - unit.targetable.target.cachedTransform.position).normalized * oppositeValue;

                    Vector2 rnd = (Random.insideUnitCircle + oppositeVector).normalized * distance;

                    goal = (Vector2)unit.cachedTransform.position + rnd;
                    oppositeValue -= 0.1f;
                } while (!BoardManager.IsPointInBoard(goal));
                
                Debug.DrawLine(unit.cachedTransform.position, goal, Color.yellow, 1);
                moveable.SetDestination(goal);
                leftDelay = unit.data.moveDelay * 0.5f;
            }
        }
    }
}
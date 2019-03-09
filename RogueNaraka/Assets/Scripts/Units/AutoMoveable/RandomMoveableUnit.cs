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
                moveable.Move((Vector2)cashedTransform.position + vec.normalized * Mathf.Min(distance, targetable.targetDistance));
            }
            else
            {
                Vector2 rnd = Random.insideUnitCircle * distance;
                moveable.Move((Vector2)cashedTransform.position + rnd);
            }
        }
    }
}
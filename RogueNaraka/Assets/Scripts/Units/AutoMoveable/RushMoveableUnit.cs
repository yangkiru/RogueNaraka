using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts.Targetable;

namespace RogueNaraka.UnitScripts.AutoMoveable
{
    public class RushMoveableUnit : AutoMoveableUnit
    {
        TargetableUnit targetable;
        public override void Init(UnitData data)
        {
            base.Init(data);
            targetable = unit.targetable;
        }

        protected override void AutoMove()
        {
            if (targetable && targetable.target)
            {
                Vector2 vec = targetable.target.cachedTransform.position - cashedTransform.position;
                moveable.SetDestination((Vector2)cashedTransform.position + vec.normalized * Mathf.Min(distance, targetable.targetDistance));
            }
        }
    }
}
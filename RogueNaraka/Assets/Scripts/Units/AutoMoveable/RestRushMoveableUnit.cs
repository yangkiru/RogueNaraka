using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts.Targetable;

namespace RogueNaraka.UnitScripts.AutoMoveable
{
    public class RestRushMoveableUnit : AutoMoveableUnit
    {
        TargetableUnit targetable;
        public override void Init(UnitData data)
        {
            base.Init(data);
            targetable = unit.targetable;
        }

        protected override void AutoMove()
        {
            if(unit.hpable.currentHp <= unit.hpable.maxHp)
            {
                if (targetable?.target)
                {
                    Vector2 vec = targetable.target.transform.position - transform.position;
                    moveable.Move((Vector2)transform.position + vec.normalized * Mathf.Min(distance, targetable.targetDistance));
                }
            }
        }
    }
}
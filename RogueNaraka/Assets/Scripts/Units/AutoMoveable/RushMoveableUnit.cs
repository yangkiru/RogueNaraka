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
            if(targetable?.target)
                moveable.Move(targetable.target.transform.position);
        }
    }
}
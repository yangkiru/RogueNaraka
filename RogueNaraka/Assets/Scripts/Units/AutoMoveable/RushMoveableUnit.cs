using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts.Targetable;

namespace RogueNaraka.UnitScripts.AutoMoveable
{
    public class RushMoveableUnit : AutoMoveableUnit
    {
        Unit unit;
        TargetableUnit targetable;
        public override void Init(UnitData data)
        {
            base.Init(data);
            unit = GetComponent<Unit>();
            targetable = unit.targetable;
        }
        protected override void AutoMove()
        {

        }
    }
}
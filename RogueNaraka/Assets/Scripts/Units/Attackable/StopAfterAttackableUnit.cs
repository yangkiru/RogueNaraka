using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts.Attackable
{
    public class StopAfterAttackableUnit : AttackableUnit
    {
        protected override void OnAfterAttackEnd()
        {
            unit.moveable.Stop();
            if (unit.autoMoveable)
                unit.autoMoveable.enabled = true;
        }

        protected override void OnAfterAttackStart()
        {
            if (unit.autoMoveable)
                unit.autoMoveable.enabled = false;
        }

        protected override void OnBeforeAttackEnd()
        {
            
        }

        protected override void OnBeforeAttackStart()
        {
            
        }
    }
}
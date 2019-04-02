using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts.Attackable
{
    public class StopBeforeAttackableUnit : AttackableUnit
    {
        protected override void OnAfterAttackEnd()
        {
            
        }

        protected override void OnAfterAttackStart()
        {
            
        }

        protected override void OnBeforeAttackEnd()
        {
            unit.moveable.Stop();
            if(unit.autoMoveable)
                unit.autoMoveable.enabled = true;
        }

        protected override void OnBeforeAttackStart()
        {
            if (unit.autoMoveable)
                unit.autoMoveable.enabled = false;
        }
    }
}

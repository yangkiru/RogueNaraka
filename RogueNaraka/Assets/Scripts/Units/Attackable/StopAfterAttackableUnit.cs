using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts.Attackable
{
    public class StopAfterAttackableUnit : AttackableUnit
    {
        protected override void OnAfterAttackEnd()
        {
            unit.moveable.agent.enabled = true;
            unit.moveable.enabled = true;
        }

        protected override void OnAfterAttackStart()
        {
            unit.moveable.agent.enabled = false;
            unit.moveable.enabled = false;
        }

        protected override void OnBeforeAttackEnd()
        {
            
        }

        protected override void OnBeforeAttackStart()
        {
            
        }
    }
}
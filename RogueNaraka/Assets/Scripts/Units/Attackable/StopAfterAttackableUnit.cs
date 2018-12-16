using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts.Attackable
{
    public class StopAfterAttackableUnit : AttackableUnit
    {
        protected override void OnAfterAttackEnd()
        {
            owner.moveable.agent.enabled = true;
        }

        protected override void OnAfterAttackStart()
        {
            owner.moveable.agent.enabled = false;
        }

        protected override void OnBeforeAttackEnd()
        {
            
        }

        protected override void OnBeforeAttackStart()
        {
            
        }
    }
}
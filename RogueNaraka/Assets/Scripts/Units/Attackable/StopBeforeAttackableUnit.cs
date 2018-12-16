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
            owner.moveable.agent.enabled = true;
        }

        protected override void OnBeforeAttackStart()
        {
            owner.moveable.agent.enabled = false;
        }
    }
}

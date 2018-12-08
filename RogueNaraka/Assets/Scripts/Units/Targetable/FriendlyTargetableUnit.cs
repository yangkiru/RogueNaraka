using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts.Targetable
{
    public class FriendlyTargetableUnit : TargetableUnit
    {
        protected override Unit GetTarget()
        {
            List<Unit> list = BoardManager.instance.friendlies;
            list.Sort(Compare);
            return list[0];
        }
    }
}
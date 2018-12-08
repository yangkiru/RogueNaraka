using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts.Targetable
{
    public class EnemyTargetableUnit : TargetableUnit
    {
        protected override Unit GetTarget()
        {
            List<Unit> list = BoardManager.instance.enemies;
            list.Sort(Compare);
            return list[0];
        }
    }
}
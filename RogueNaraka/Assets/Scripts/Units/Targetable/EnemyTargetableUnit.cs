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
            for(int i = list.Count - 1; i >= 0; i--)
            {
                if (!list[i].targetable.isTargetable)
                    list.RemoveAt(i);
            }
            if (list.Count == 0)
                return null;
            Unit min = list[0];

            float minDistance;
            minDistance = Distance(list[0]);
            for (int i = 1; i < list.Count; i++)
            {
                float newDistance = Distance(list[i]);
                if (minDistance > newDistance)
                {
                    min = list[i];
                    minDistance = newDistance;
                }
            }
            return min;
        }
    }
}
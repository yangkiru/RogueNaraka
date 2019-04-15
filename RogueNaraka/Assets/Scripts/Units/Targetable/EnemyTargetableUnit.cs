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
            if (list.Count == 0)
                return null;
            Unit min = null;
            float minDistance = 0;
            bool isFirst = true;
            for (int i = 0; i < list.Count; i++)
            {
                if (isFirst)
                {
                    if (list[i].targetable.IsTargetable)
                    {
                        min = list[i];
                        minDistance = Distance(list[i]);
                        isFirst = false;
                        continue;
                    }
                    else
                    {
                        continue;
                    }

                }

                if (!list[i].targetable.IsTargetable)
                    continue;

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
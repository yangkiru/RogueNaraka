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
            if (list.Count == 0)
                return null;
            Unit min = list[0];
            float minDistance = Distance(list[0]);
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.Bullet
{
    public class HitableCircle : Hitable
    {
        float distance;
        float angle;
        Vector2 size;
        Vector2 castDirection = Vector2.zero;
        CapsuleDirection2D circleDirection = CapsuleDirection2D.Horizontal;
        

        public override void GetHitUnits(List<Unit> hitUnitList)
        {
            RaycastHit2D[] hits;
            hits = Physics2D.CapsuleCastAll(transform.position, size, circleDirection, angle, castDirection, distance);
            for(int i = 0; i < hits.Length; i++)
            {
                if(CheckHitList(hits[i].GetHashCode()))
                    hitUnitList.Add(hits[i].transform.GetComponent<Unit>());
            }
        }
    }
}

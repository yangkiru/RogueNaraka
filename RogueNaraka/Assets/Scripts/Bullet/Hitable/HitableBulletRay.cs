﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.BulletScripts.Hitable
{
    public class HitableBulletRay : HitableBullet
    {
        [SerializeField]
        Vector2 offset;
        [SerializeField]
        Vector2 direction;
        [SerializeField]
        float distance;
        public override void Init(NewBulletData data)
        {
            base.Init(data);
        }

        public override void GetHitUnits(List<OldUnit> hitUnitList)
        {
            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll((Vector2)transform.position + offset, direction, distance, layerMask);
            for (int i = 0; i < hits.Length; i++)
            {
                if (CheckHitList(hits[i].GetHashCode()))
                    hitUnitList.Add(hits[i].transform.GetComponent<OldUnit>());
            }
        }
    }
}

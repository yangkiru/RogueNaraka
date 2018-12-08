using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.BulletScripts.Hitable
{
    public class HitableBulletCircle : HitableBullet
    {
        [SerializeField]
        float size;

        public override void Init(NewBulletData data)
        {
            base.Init(data);
            size = data.size;
        }

        public override void GetHitUnits(List<OldUnit> hitUnitList)
        {
            RaycastHit2D[] hits;
            hits = Physics2D.CircleCastAll(transform.position, size, Vector2.zero, 0, layerMask);
            for(int i = 0; i < hits.Length; i++)
            {
                if(CheckHitList(hits[i].GetHashCode()))
                    hitUnitList.Add(hits[i].transform.GetComponent<OldUnit>());
            }
        }
    }
}

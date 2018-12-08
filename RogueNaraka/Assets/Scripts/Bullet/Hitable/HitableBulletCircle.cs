using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.Bullet
{
    public class HitableBulletCircle : HitableBullet
    {
        [SerializeField]
        float size;

        public override void Init(NewBulletData data)
        {
            size = data.size;
            layerMask = GetlayerMask();
        }

        public override void GetHitUnits(List<Unit> hitUnitList)
        {
            RaycastHit2D[] hits;
            hits = Physics2D.CircleCastAll(transform.position, size, Vector2.zero, 0, layerMask);
            for(int i = 0; i < hits.Length; i++)
            {
                if(CheckHitList(hits[i].GetHashCode()))
                    hitUnitList.Add(hits[i].transform.GetComponent<Unit>());
            }
        }
    }
}

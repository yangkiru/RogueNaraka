using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.Bullet
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
            throw new System.NotImplementedException();
        }

        public override void GetHitUnits(List<Unit> hitUnitList)
        {
            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll((Vector2)transform.position + offset, direction, distance, layerMask);
            for (int i = 0; i < hits.Length; i++)
            {
                if (CheckHitList(hits[i].GetHashCode()))
                    hitUnitList.Add(hits[i].transform.GetComponent<Unit>());
            }
        }
    }
}

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

        public override void Init(BulletData data)
        {
            base.Init(data);
            size = data.size;
        }

        protected override void GetHitUnits()
        {
            RaycastHit2D[] hits;
            hits = Physics2D.CircleCastAll(transform.position, size, Vector2.zero, 0, layerMask);
            Debug.Log(hits.Length);
            for(int i = 0; i < hits.Length; i++)
            {
                Unit hit = hits[i].transform.GetComponent<Unit>();
                AddHitList(hit);
            }
            if (hits.Length > 0)
                SubPierce();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos. DrawWireSphere(transform.position, size);
        }
    }
}

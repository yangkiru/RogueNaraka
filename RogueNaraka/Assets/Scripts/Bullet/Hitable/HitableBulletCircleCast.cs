using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.BulletScripts.Hitable
{
    public class HitableBulletCircleCast : HitableBullet
    {
        protected override void Update()
        {
            base.Update();
            RaycastHit2D[] hits;
            hits = Physics2D.CircleCastAll(transform.position, bullet.data.size, Vector2.zero, 0, layerMask);
            for (int i = 0; i < hits.Length; i++)
            {
                Hit(hits[i].collider);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, bullet.data.size);
        }
    }
}

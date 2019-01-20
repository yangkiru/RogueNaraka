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
            hits = Physics2D.CircleCastAll(bullet.cachedTransform.position, bullet.data.size, Vector2.zero, 0, layerMask);
            int length = hits.Length;
            for (int i = 0; i < hits.Length; i++)
            {
                Hit(hits[i].collider);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (isDestroy)
                return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(bullet.cachedTransform.position, bullet.data.size);
        }
#endif
    }
}

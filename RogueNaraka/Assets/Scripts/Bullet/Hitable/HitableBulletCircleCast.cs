using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.BulletScripts.Hitable
{
    public class HitableBulletCircleCast : HitableBullet
    {

        public override void OnSpawn()
        {
            //Debug.Log("OnSpawn" + name);
            StartCoroutine(CheckCorou());
            //CheckHit();
        }

        IEnumerator CheckCorou()
        {
            while (true)
            {
                if (leftDelay <= 0)
                {
                    //Debug.Log("CheckHitCorou");
                    CheckHit();
                    yield return new WaitForFixedUpdate();
                }
                else
                {
                    yield return null;
                }
            }
        }
        private void CheckHit()
        {
            RaycastHit2D[] hits;
            hits = Physics2D.CircleCastAll(bullet.cachedTransform.position, bullet.data.size, Vector2.zero, 0, layerMask);
            for (int i = 0; i < hits.Length; i++)
            {
                //Debug.Log("!!!!!!!!!!hit " + hits[i].collider.name);
                HIT result = Hit(hits[i].collider);
                switch(result)
                {
                    case HIT.ENEMY:
                        //Debug.Log("Hit Enemy:" + name);
                        if(pierce < 99999)
                            pierce--;
                        if (!isSplash)
                            CheckPierce();
                        break;
                    case HIT.WALL:
                        //Debug.Log("Hit Wall:" + name + ", " + hits[i].collider.name + " " + hits[i].point);
                        
                        if (!isHitableWall)
                        {
                            pierce = 0;
                            if (!isSplash)
                                CheckPierce();
                        }
                        break;
                    //case HIT.FRIENDLY:
                    //    Debug.Log("Hit Friendly:"+name);
                    //    break;
                    //case HIT.ETC:
                    //    Debug.Log("Hit ETC:" + name);
                    //    break;
                }
            }
            if (isSplash)
                CheckPierce();
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

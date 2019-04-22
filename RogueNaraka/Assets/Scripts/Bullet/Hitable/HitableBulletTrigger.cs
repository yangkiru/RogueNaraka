using UnityEngine;
using System.Collections;

namespace RogueNaraka.BulletScripts.Hitable
{
    public class HitableBulletTrigger : HitableBullet
    {
        bool isPoly;
        bool isCircle;

        public override void Init(BulletData data)
        {
            base.Init(data);
            switch (data.type)
            {
                case BULLET_TYPE.DYNAMIC_CIRCLE:
                    bullet.circle.enabled = true;
                    bullet.polygon.enabled = false;
                    bullet.circle.radius = data.size;
                    break;
                case BULLET_TYPE.DYNAMIC_POLY:
                    bullet.circle.enabled = false;
                    bullet.polygon.enabled = true;
                    break;
                default:
                    break;
            }
        }

        protected override void OnHit()
        {
            if (!isPoly && !isCircle)
            {
                if (bullet.polygon.enabled)
                {
                    bullet.polygon.enabled = false;
                    isPoly = true;
                }
                else if (bullet.circle.enabled)
                {
                    bullet.circle.enabled = false;
                    isCircle = true;
                }
            }
            else if (isPoly)
                bullet.polygon.enabled = false;
            else if (isCircle)
                bullet.circle.enabled = false;
        }

        protected override void OnNotHit()
        {
            if (isPoly)
                bullet.polygon.enabled = true;
            else if (isCircle)
                bullet.circle.enabled = true;
        }

        private void OnTriggerStay2D(Collider2D coll)
        {
            //Debug.Log(name + " OnTriggerStay " + coll.name);
            if (pierce <= 0 && leftDelay > 0)
                return;
            HIT result = Hit(coll);
            switch(result)
            {
                case HIT.ENEMY:
                    //Debug.Log("Hit Enemy:" + name);
                    if(pierce < 99999)
                        pierce--;
                    if (isSplash)
                    {
                        if (pierceCorou == null)
                        {
                            pierceCorou = PierceCorou();
                            StartCoroutine(pierceCorou);
                        }
                        
                    }
                    else
                        CheckPierce();
                    break;
                case HIT.WALL:
                    //Debug.Log("Hit Wall:" + name);
                    if (!isHitableWall)
                    {
                        pierce = 0;
                        StartCoroutine(PierceCorou());
                    }
                    break;
            }
        }

        IEnumerator pierceCorou;

        IEnumerator PierceCorou()
        {
            yield return new WaitForFixedUpdate();
            CheckPierce();
            pierceCorou = null;
        }
    }
}
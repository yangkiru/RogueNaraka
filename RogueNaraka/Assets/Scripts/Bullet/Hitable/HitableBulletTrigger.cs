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
                    bullet.advanced.enabled = false;
                    bullet.circle.radius = data.size;
                    break;
                case BULLET_TYPE.DYNAMIC_POLY:
                    bullet.circle.enabled = false;
                    bullet.polygon.enabled = true;
                    bullet.advanced.enabled = true;
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
            Hit(coll);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;
using RogueNaraka.BulletScripts.Hitable;

namespace RogueNaraka.BulletScripts
{
    public class Bullet : MonoBehaviour
    {
        HitableBullet hitableRay = null;
        HitableBullet hitableCircle = null;
        HitableBullet hitable = null;

        ShootableBullet shootable = null;
        MoveableBullet moveable = null;
        OwnerableBullet ownerable = null;

        void Awake()
        {
            hitableRay = GetComponent<HitableBulletRay>();
            hitableCircle = GetComponent<HitableBulletCircle>();
            moveable = GetComponent<MoveableBullet>();
            shootable = GetComponent<ShootableBullet>();
        }

        public void Init(OldUnit owner, NewBulletData data)
        {
            ownerable.SetOwner(owner);

            //Hitable
            DisableAllHitable();
            switch(data.type)
            {
                case BULLET_TYPE.CIRCLECAST:
                    hitable = hitableCircle;
                    break;
                case BULLET_TYPE.RAYCAST:
                    hitable = hitableRay;
                    break;
            }
            hitable.enabled = true;
            hitable.Init(data);
        }

        void DisableAllHitable()
        {
            hitableRay.enabled = false;
            hitableCircle.enabled = false;
        }

        public void Spawn(OldUnit owner, NewBulletData data, Vector2 position)
        {
            Init(owner, data);
            transform.position = position;
        }

        public void Spawn(Vector3 position)
        {
            transform.position = position;
        }
    }
}

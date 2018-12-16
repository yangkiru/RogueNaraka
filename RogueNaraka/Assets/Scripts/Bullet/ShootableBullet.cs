using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.BulletScripts
{
    public class ShootableBullet : MonoBehaviour
    {
        MoveableBullet moveable;
        void Awake()
        {
            moveable = GetComponent<MoveableBullet>();
        }

        public void Shoot(Vector3 direction, float localSpeed, float worldSpeed, float localAccel, float worldAccel)
        {
            float angle = 180 + Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = q;
            moveable.SetVelocity(Vector2.left * localSpeed, Vector2.left * localAccel, Space.Self);
            moveable.SetVelocity(Vector2.left * worldSpeed, Vector2.left * worldAccel, Space.World);
        }
    }
}
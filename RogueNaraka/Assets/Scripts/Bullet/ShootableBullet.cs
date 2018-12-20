using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.BulletScripts
{
    public class ShootableBullet : MonoBehaviour
    {
        [SerializeField]
        MoveableBullet moveable;
        void Reset()
        {
            moveable = GetComponent<MoveableBullet>();
        }

        public void Shoot(Vector3 direction, Vector3 offset, float localSpeed, float worldSpeed, float localAccel, float worldAccel)
        {
            float angle = 180 + Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = q;

            transform.position += transform.up * offset.x + transform.right * -offset.y + transform.forward * offset.z;

            moveable.SetVelocity(Vector2.left * localSpeed, Vector2.left * localAccel, Space.Self);
            moveable.SetVelocity(Vector2.left * worldSpeed, Vector2.left * worldAccel, Space.World);
        }
    }
}
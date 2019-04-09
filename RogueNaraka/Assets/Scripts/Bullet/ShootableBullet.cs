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

        public void Shoot(Vector3 direction, Vector3 offset, float localSpeed, float worldSpeed, float localAccel, float worldAccel, bool isRotate = true)
        {
            direction = direction.normalized;
            if (isRotate)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = q;
            }

            if(offset != Vector3.zero)
                transform.position += transform.up * offset.x + transform.right * offset.y + transform.forward * offset.z;

            moveable.SetVelocity(Vector2.right * localSpeed, Vector2.right * localAccel, Space.Self);
            moveable.SetVelocity(direction * worldSpeed, direction * worldAccel, Space.World);
        }

        public void Shoot(Quaternion q, Vector3 offset, float localSpeed, float worldSpeed, float localAccel, float worldAccel, bool isRotate = true)
        {
            if(isRotate)
                transform.rotation = q;

            Vector3 direction = q.eulerAngles.normalized;

            if (offset != Vector3.zero)
                transform.position += transform.up * offset.x + transform.right * offset.y + transform.forward * offset.z;

            moveable.SetVelocity(Vector2.right * localSpeed, Vector2.right * localAccel, Space.Self);
            moveable.SetVelocity(direction * worldSpeed, direction * worldAccel, Space.World);
        }
    }
}
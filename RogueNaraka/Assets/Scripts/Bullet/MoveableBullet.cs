using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.BulletScripts
{
    public class MoveableBullet : MonoBehaviour
    {
        [SerializeField]
        Bullet bullet;
        [SerializeField]
        Vector3 localVelocity;
        [SerializeField]
        Vector3 worldVelocity;
        [SerializeField]
        Vector3 localAccel;
        [SerializeField]
        Vector3 worldAccel;

        private void Reset()
        {
            bullet = GetComponent<Bullet>();
        }

        public void Init()
        {
            localAccel = Vector3.zero;
            worldAccel = Vector3.zero;
            localVelocity = Vector3.zero;
            worldVelocity = Vector3.zero;
        }

        public void SetVelocity(Vector3 velocity, Space space)
        {
            if (space == Space.Self)
                localVelocity = velocity;
            else
                worldVelocity = velocity;
        }

        public void SetVelocity(Vector3 velocity, Vector3 accel, Space space)
        {
            if (space == Space.Self)
            {
                localVelocity = velocity;
                localAccel = accel;
            }
            else
            {
                worldVelocity = velocity;
                worldAccel = accel;
            }
        }

        public void SetAccel(Vector3 accel, Space space)
        {
            if (space == Space.Self)
                localAccel = accel;
            else
                worldAccel = accel;
        }

        private void Update()
        {
            localVelocity += localAccel;
            worldVelocity += worldAccel;
            bullet.rigid.velocity = worldVelocity + transform.TransformDirection(localVelocity);
        }
    }
}

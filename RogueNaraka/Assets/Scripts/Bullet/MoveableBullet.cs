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

        //public void SetVelocity(Vector3 velocity, Space space)
        //{
        //    if (space == Space.Self)
        //        localVelocity = velocity;
        //    else
        //    {
        //        direction = bullet.cachedTransform.forward;
        //        worldVelocity = velocity;
        //    }
        //}

        public Vector3 GetVelocity(Space space)
        {
            return space == Space.Self ? localVelocity : worldVelocity;
        }

        public Vector3 GetAccel(Space space)
        {
            return space == Space.Self ? localAccel : worldAccel;
        }

        public void GetValues(out Vector3 localVelocity, out Vector3 worldVelocity, out Vector3 localAccel, out Vector3 worldAccel)
        {
            localVelocity = this.localVelocity;
            worldVelocity = this.worldVelocity;
            localAccel = this.localAccel;
            worldAccel = this.worldAccel;
        }
        public void SetVelocity(Vector3 velocity, Vector3 accel, Space space)
        {
            //SetVelocity(velocity, space);
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

        //private void OnDrawGizmos()
        //{
        //    Gizmos.DrawRay(bullet.cachedTransform.position, worldVelocity);
        //}

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
            Vector3 velocity = new Vector3(worldVelocity.x, worldVelocity.y, worldVelocity.z * worldVelocity.z);
            
            bullet.rigid.velocity = velocity + bullet.cachedTransform.TransformDirection(localVelocity);
        }
    }
}

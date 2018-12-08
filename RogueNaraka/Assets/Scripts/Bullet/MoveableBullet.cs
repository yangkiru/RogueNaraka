using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.BulletScripts
{
    public class MoveableBullet : MonoBehaviour
    {
        /// <summary>
        /// 매 프레임 속력 증가 값
        /// </summary>
        [SerializeField]
        Vector3 accel;
        [SerializeField]
        Vector3 velocity;

        public void SetVelocity(Vector3 velocity)
        {
            this.velocity = velocity;
        }

        public void SetVelocity(Vector3 velocity, Vector3 accel)
        {
            this.velocity = velocity;
            this.accel = accel;
        }

        public void SetAccel(Vector3 accel)
        {
            this.accel = accel;
        }

        private void Update()
        {
            velocity += accel;
            transform.Translate(velocity * Time.deltaTime, Space.Self);
        }
    }
}

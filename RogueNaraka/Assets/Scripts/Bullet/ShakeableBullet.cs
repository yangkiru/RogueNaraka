using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace RogueNaraka.BulletScripts
{
    public class ShakeableBullet : MonoBehaviour
    {
        public Bullet bullet;

        private void Reset()
        {
            bullet = GetComponent<Bullet>();
        }

        public void Shake()
        {
            GameManager.instance.ShakeCamera(bullet.data.shake.power, bullet.data.shake.time, bullet.data.shake.gap);
            //CameraShake.instance.Shake(bullet.data.shake);
        }
    }
}
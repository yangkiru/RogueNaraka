using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            CameraShake.instance.Shake(bullet.data.shake);
        }
    }
}
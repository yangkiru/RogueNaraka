using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.BulletScripts
{
    public class ShakeableBullet : MonoBehaviour
    {
        public ShakeData shake { get { return _shake; } }
        ShakeData _shake;

        public void Init(ShakeData data)
        {
            _shake.time = data.time;
            _shake.gap = data.gap;
            _shake.power = data.power;
            _shake.isOnHit = data.isOnHit;
        }

        public void Shake()
        {
            CameraShake.instance.Shake(_shake);
        }
    }
}
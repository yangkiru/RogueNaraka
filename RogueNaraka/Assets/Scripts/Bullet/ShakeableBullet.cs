using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.BulletScripts
{
    public class ShakeableBullet : MonoBehaviour
    {
        public ShakeData shake { get { return _shake; } }
        ShakeData _shake;
        float leftTime;

        public void Init(ShakeData data)
        {
            _shake.time = data.time;
            _shake.gap = data.gap;
            _shake.power = data.power;
            _shake.isOnHit = data.isOnHit;
            leftTime = 0;
        }

        public void Shake()
        {
            CameraShake.instance.Shake(_shake);
        }

        private void Update()
        {
            if (_shake.isOnHit)
                return;
            if(leftTime > 0)
            {
                leftTime -= Time.deltaTime;
                return;
            }
            Shake();
            leftTime = _shake.time;
        }
    }
}
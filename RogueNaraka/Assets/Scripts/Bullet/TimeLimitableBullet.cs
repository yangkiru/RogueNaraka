using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RogueNaraka.BulletScripts
{
    public class TimeLimitableBullet : MonoBehaviour
    {
        [SerializeField]
        Bullet bullet;
        float time;
        float leftTime;

        public void Init(BulletData data)
        {
            time = data.limitTime;
            leftTime = time;
        }

        private void Update()
        {
            if (leftTime > 0)
                leftTime -= Time.deltaTime;
            else
            {
                bullet.Destroy();
                enabled = false;
            }
        }

        private void Reset()
        {
            bullet = GetComponent<Bullet>();
        }
    }
}
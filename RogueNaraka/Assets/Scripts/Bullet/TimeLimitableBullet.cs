using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RogueNaraka.BulletScripts
{
    public class TimeLimitableBullet : MonoBehaviour
    {
        [SerializeField]
        Bullet bullet;
        [SerializeField]
        float time;
        [SerializeField]
        float leftTime;

        private void Reset()
        {
            bullet = GetComponent<Bullet>();
        }

        public void Init(BulletData data)
        {
            time = data.limitTime;
            leftTime = time;
        }

        private void Update()
        {
            if (leftTime > 0)
                leftTime -= Time.deltaTime;
            else if (time != 0)
            {
                bullet.Destroy();
                enabled = false;
            }
        }
    }
}
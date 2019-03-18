using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.BulletScripts
{
    public class DisapearableBullet : MonoBehaviour
    {
        [SerializeField]
        Bullet bullet;

        public float startTime { get { return _startTime; } }
        float _startTime;
        public float duration { get { return _duration; } }
        float _duration;

        private void Reset()
        {
            bullet = GetComponent<Bullet>();
        }

        public void Init(BulletData data)
        {
            _startTime = data.disapearStartTime;
            _duration = data.disapearDuration;
        }

        public void Disapear()
        {
            StartCoroutine(DisapearCorou());
        }

        IEnumerator DisapearCorou()
        {
            Color color = bullet.renderer.color;
            float t = 0;
            while (t < _startTime)
            {
                t += Time.deltaTime;
                yield return null;
            }

            float alpha = bullet.renderer.color.a;

            t = 0;

            while (t < 1)
            {
                yield return null;
                float amount = Time.deltaTime / _duration;
                color.a -= amount;
                t += amount;
                bullet.renderer.color = color;
            }
            color.a = 0;
            bullet.renderer.color = color;
        }
    }
}
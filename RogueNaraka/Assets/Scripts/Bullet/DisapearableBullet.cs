using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.BulletScripts
{
    public class DisapearableBullet : MonoBehaviour
    {
        [SerializeField]
        Bullet bullet;

        private void Reset()
        {
            bullet = GetComponent<Bullet>();
        }

        public IEnumerator Disapear(float time, float wait)
        {
            Color color = bullet.renderer.color;
            yield return new WaitForSeconds(wait);
            float alpha = bullet.renderer.color.a;

            float t = 0;

            while (t < 1)
            {
                yield return null;
                color.a -= alpha * Time.deltaTime;
                t += Time.deltaTime / time;
                bullet.renderer.color = color;
            }
            color.a = 0;
            bullet.renderer.color = color;
        }
    }
}
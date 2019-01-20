using UnityEngine;
using System.Collections;

namespace RogueNaraka.BulletScripts
{
    public class SpinableBullet : MonoBehaviour
    {
        [SerializeField]
        Bullet bullet;

        float speed;
        private void Reset()
        {
            bullet = GetComponent<Bullet>();
        }

        public void Init(BulletData data)
        {
            speed = data.spinSpeed;
        }
        
        void Update()
        {
            bullet.cachedTransform.rotation = Quaternion.Euler(0, 0, bullet.cachedTransform.rotation.eulerAngles.z + speed * Time.deltaTime);
        }
    }
}
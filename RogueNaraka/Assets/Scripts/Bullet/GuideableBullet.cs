using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;
using RogueNaraka.UnitScripts.Targetable;

namespace RogueNaraka.BulletScripts
{
    public class GuideableBullet : MonoBehaviour
    {
        [SerializeField]
        Bullet bullet;
        [SerializeField]
        Unit target;

        public float rotateSpeed { get { return _rotateSpeed; } }
        float _rotateSpeed;

        private void Reset()
        {
            bullet = GetComponent<Bullet>();
        }

        public void Init(BulletData data)
        {
            _rotateSpeed = data.guideSpeed;
        }

        void FixedUpdate()
        {
            target = bullet.ownerable.unit.targetable.target;
            if (!target)
                return;
            Vector2 direction = target.transform.position - transform.position;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotateToTarget = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotateToTarget, Time.deltaTime * _rotateSpeed);
        }
    }
}
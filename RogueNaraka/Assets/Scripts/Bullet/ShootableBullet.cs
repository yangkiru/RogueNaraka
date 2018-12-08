using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.Bullet
{
    public class ShootableBullet : MonoBehaviour
    {
        MoveableBullet moveable;
        void Awake()
        {
            moveable = GetComponent<MoveableBullet>();
        }

        public void Shoot(Vector3 direction, float speed, float accel)
        {
            Vector3 normalized = direction.normalized;
            moveable.SetVelocity(normalized * speed, normalized * accel);
        }
    }
}
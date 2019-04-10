using UnityEngine;
using System.Collections;

namespace RogueNaraka.BulletScripts
{
    public class BounceableBullet : MonoBehaviour
    {
        public Bullet bullet;

        public enum DIRECTION { UP, RIGHT, DOWN, LEFT}
        private void Reset()
        {
            bullet = GetComponent<Bullet>();
        }

        public void Bounce(DIRECTION direction)
        {
            Vector3 localV;
            Vector3 worldV;
            Vector3 localA;
            Vector3 worldA;

            Vector3 related = Vector3.zero;

            bullet.moveable.GetValues(out localV, out worldV, out localA, out worldA);

            switch (direction)
            {
                case DIRECTION.UP:
                    related = Vector2.up;
                    break;
                case DIRECTION.RIGHT:
                    related = Vector2.right;
                    break;
                case DIRECTION.DOWN:
                    related = Vector2.down;
                    break;
                case DIRECTION.LEFT:
                    related = Vector2.left;
                    break;
            }

            localV = BouncedVector(localV, related);
            worldV = BouncedVector(worldV, related);
            localA = BouncedVector(localA, related);
            worldA = BouncedVector(worldA, related);

            bullet.moveable.SetVelocity(localV, localA, Space.Self);
            bullet.moveable.SetVelocity(worldV, worldA, Space.World);

        }

        public static Vector3 BouncedVector(Vector3 input, Vector3 related)
        {
            related.x = (input.x > 0 && related.x > 0) || (input.x < 0 && related.x < 0) ? -1 : 1;
            related.y = (input.y > 0 && related.y > 0) || (input.y < 0 && related.y < 0) ? -1 : 1;
            related.z = (input.z > 0 && related.z > 0) || (input.z < 0 && related.z < 0) ? -1 : 1;
            return new Vector3(input.x * related.x, input.y * related.y, input.z * related.z);
        }
    }  
}

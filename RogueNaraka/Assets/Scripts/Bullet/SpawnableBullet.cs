using RogueNaraka.TimeScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.BulletScripts
{
    public class SpawnableBullet : MonoBehaviour
    {
        [SerializeField]
        Bullet bullet;

        public List<Bullet> destroyChildList { get { return _destroyChildList; } }
        List<Bullet> _destroyChildList = new List<Bullet>();

        public List<BulletChildData> onDestroyList { get { return _onDestroyList; } }
        List<BulletChildData> _onDestroyList = new List<BulletChildData>();

        void Reset()
        {
            bullet = GetComponent<Bullet>();
        }

        public void Init(BulletData data)
        {
            _destroyChildList.Clear();
            _onDestroyList.Clear();
            for (int i = 0; i < data.children.Length; i++)
            {
                BulletInit(data.children[i]);
            }
            for (int i = 0; i < data.onDestroy.Length; i++)
            {
                _onDestroyList.Add(data.onDestroy[i]);
            }
        }

        public void OnDestroyBullet()
        {
            for(int i = 0; i < _onDestroyList.Count; i++)
            {
                BulletInit(_onDestroyList[i]);
            }
        }


        void BulletInit(BulletChildData data)
        {
            Bullet child = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
            child.Init(bullet.ownerable.unit, GameDatabase.instance.bullets[data.bulletId]);
            //child.renderer.sortingOrder = bullet.renderer.sortingOrder + data.sortingOrder;
            child.spawnable.StartCoroutine(child.spawnable.BulletSpawn(bullet, data));
        }

        public IEnumerator BulletSpawn(Bullet parent, BulletChildData data)
        {
            float t = data.startTime;

            do
            {
                yield return null;
                t -= TimeManager.Instance.DeltaTime;
            } while (t > 0);

            bullet.Spawn(parent.cachedTransform.position);
            bullet.renderer.color = parent.renderer.color;

            if (data.isDestroyWith)
                parent.spawnable.destroyChildList.Add(parent);

            if (data.isStick)
            {
                transform.SetParent(parent.cachedTransform);
            }

            bullet.cachedTransform.rotation = parent.cachedTransform.rotation;

            t = data.waitTime;

            do
            {
                yield return null;
                t -= TimeManager.Instance.DeltaTime;
            } while (t > 0);

            if (data.angle >= 0)
            {
                Vector3 direction = Quaternion.AngleAxis(data.angle, Vector3.back) * bullet.cachedTransform.rotation.eulerAngles;
                bullet.shootable.Shoot(direction.normalized, data.offset, bullet.data.localSpeed, bullet.data.worldSpeed, bullet.data.localAccel, bullet.data.worldAccel);
            }
            else
                bullet.shootable.Shoot(parent.cachedTransform.rotation, data.offset, bullet.data.localSpeed, bullet.data.worldSpeed, bullet.data.localAccel, bullet.data.worldAccel);
            if (data.isRepeat && parent.gameObject.activeSelf)
                parent.spawnable.BulletInit(data);
        }
    }
}
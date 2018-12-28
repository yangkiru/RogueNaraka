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
            child.Init(bullet.ownerable.owner, GameDatabase.instance.bullets[data.bulletId]);
            child.renderer.sortingOrder = bullet.renderer.sortingOrder + data.sortingOrder;
            child.spawnable.StartCoroutine(child.spawnable.BulletSpawn(bullet, data));
        }

        public IEnumerator BulletSpawn(Bullet parent, BulletChildData data)
        {
            yield return new WaitForSeconds(data.startTime);

            bullet.Spawn(parent.transform.position);
            if (data.isDestroyWith)
                parent.spawnable.destroyChildList.Add(parent);

            if (data.isStick)
                transform.SetParent(parent.transform);

            yield return new WaitForSeconds(data.waitTime);

            Vector3 direction = Quaternion.AngleAxis(data.angle, Vector3.back) * bullet.transform.rotation.eulerAngles;
            bullet.shootable.Shoot(direction.normalized, data.offset, bullet.data.localSpeed, bullet.data.worldSpeed, bullet.data.localAccel, bullet.data.worldAccel);

            if (data.isRepeat && parent.gameObject.activeSelf)
                parent.spawnable.BulletInit(data);
        }
    }
}
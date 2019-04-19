using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.BulletScripts;
using RogueNaraka.UnitScripts.Targetable;

namespace RogueNaraka.UnitScripts.Attackable
{
    public abstract class AttackableUnit : MonoBehaviour
    {

        public WeaponData weapon { get { return _weapon; } }
        [SerializeField]
        WeaponData _weapon;
        [SerializeField]
        protected Unit unit;

        float targetDistance { get { return unit.targetable?.target ? unit.targetable.targetDistance : float.PositiveInfinity; } }

        IEnumerator beforeAttackCorou;
        IEnumerator afterAttackCorou;

        float beforeDelay;
        float afterDelay;

        bool isBeforeAnimation;
        bool isAfterAnimation;

        void Reset()
        {
            unit = GetComponent<Unit>();
        }

        public void Init(UnitData data)
        {
            beforeAttackCorou = null;
            afterAttackCorou = null;
            WeaponData weapon = (WeaponData)GameDatabase.instance.weapons[data.weapon].Clone();
            Init(weapon);
        }

        public void Init(WeaponData data)
        {
            _weapon = data;

            beforeDelay = data.beforeAttackDelay;
            afterDelay = data.afterAttackDelay;

            isBeforeAnimation = false;
            isAfterAnimation = false;
        }

        protected virtual void Attack()
        {
            if (unit.targetable.direction == Vector2.zero)
                return;
            Bullet bullet = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();

            BulletData data = (BulletData)GameDatabase.instance.bullets[_weapon.startBulletId].Clone();

            List<BulletChildData> list = new List<BulletChildData>();

            for(int i = 0; i < data.children.Length; i++)
            {
                list.Add(data.children[i]);
            }
            for (int i = 0; i < _weapon.children.Length; i++)
            {
                list.Add(_weapon.children[i]);
            }

            data.children = list.ToArray();

            list.Clear();

            for (int i = 0; i < data.onDestroy.Length; i++)
            {
                list.Add(data.onDestroy[i]);
            }
            for (int i = 0; i < _weapon.onDestroy.Length; i++)
            {
                list.Add(_weapon.onDestroy[i]);
            }

            data.onDestroy = list.ToArray();

            bullet.Spawn(unit, data, transform.position);
            bullet.shootable.Shoot(unit.targetable.direction, _weapon.offset, bullet.data.localSpeed, bullet.data.worldSpeed, bullet.data.localAccel, bullet.data.worldAccel);
            afterAttackCorou = AfterAttack();
            StartCoroutine(afterAttackCorou);
        } 

        IEnumerator BeforeAttack()
        {
            float leftDelay = beforeDelay;
            if(isBeforeAnimation)
                unit.animator.SetBool("isBeforeAttack", true);
            bool isMoveForced = false;
            if (unit.autoMoveable && unit.autoMoveable.enabled)
                OnBeforeAttackStart();
            else
                isMoveForced = true;

            do
            {
                yield return null;
                leftDelay -= unit.isStun ? 0 : 1 * Mathf.Max(Time.deltaTime * (1 + unit.stat.GetCurrent(STAT.SPD) * 0.1f), 0);
            } while (leftDelay > 0);
            if (!isMoveForced && isBeforeAnimation)
                unit.animator.SetBool("isBeforeAttack", false);

            OnBeforeAttackEnd();

            Attack();
            beforeAttackCorou = null;
        }

        IEnumerator AfterAttack()
        {
            float leftDelay = afterDelay;
            if (isAfterAnimation)
                unit.animator.SetBool("isAfterAttack", true);

            bool isMoveForced = false;

            if (unit.autoMoveable && unit.autoMoveable.enabled)
                OnAfterAttackStart();
            else
                isMoveForced = true;

            do
            {
                yield return null;
                leftDelay -= unit.isStun ? 0 : 1 * Time.deltaTime * (1 + unit.stat.GetCurrent(STAT.SPD) * 0.1f);
            } while (leftDelay > 0);

            if (!isMoveForced && isAfterAnimation)
                unit.animator.SetBool("isAfterAttack", false);

            OnAfterAttackEnd();

            afterAttackCorou = null;
        }

        private void OnEnable()
        {
            AnimatorControllerParameter[] parameters = unit.animator.parameters;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].name.CompareTo("isBeforeAttack") == 0)
                    isBeforeAnimation = true;
                else if (parameters[i].name.CompareTo("isAfterAttack") == 0)
                    isAfterAnimation = true;
            }
        }

        private void Update()
        {
            if (unit.targetable.target && (_weapon.attackDistance == 0 || targetDistance <= _weapon.attackDistance))
            {
                if (beforeAttackCorou == null && afterAttackCorou == null)
                {
                    beforeAttackCorou = BeforeAttack();
                    StartCoroutine(beforeAttackCorou);
                }
                else if (beforeAttackCorou == null)
                    LookTarget();
            }
        }

        protected void LookTarget()
        {
            //Debug.Log("LookTarget");
            unit.animator.SetFloat("x", unit.targetable.direction.x);
            unit.animator.SetFloat("y", unit.targetable.direction.y);
            unit.animator.SetBool("isWalk", true);
        }


        protected abstract void OnBeforeAttackStart();
        protected abstract void OnBeforeAttackEnd();
        protected abstract void OnAfterAttackStart();
        protected abstract void OnAfterAttackEnd();
    }
}
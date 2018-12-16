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
        protected Unit owner;

        float targetDistance { get { return owner.targetable?.target ? owner.targetable.targetDistance : float.PositiveInfinity; } }

        IEnumerator beforeAttackCorou;
        IEnumerator afterAttackCorou;

        float beforeDelay;
        float afterDelay;

        bool isBeforeAnimation;
        bool isAfterAnimation;

        void Reset()
        {
            owner = GetComponent<Unit>();
        }

        public void Init(UnitData data)
        {
            WeaponData weapon = GameDatabase.instance.weapons[data.weapon];
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

        void Attack()
        {
            Bullet bullet = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
            bullet.Spawn(owner, GameDatabase.instance.bullets[_weapon.startBulletId], transform.position + _weapon.offset);
            bullet.shootable.Shoot(owner.targetable.direction, bullet.data.localSpeed, bullet.data.worldSpeed, bullet.data.localAccel, bullet.data.worldAccel);
        } 

        IEnumerator BeforeAttack()
        {
            float leftDelay = beforeDelay;
            if(isBeforeAnimation)
                owner.animator.SetBool("isBeforeAttack", true);
            OnBeforeAttackStart();

            do
            {
                yield return null;
                leftDelay -= Time.deltaTime;
            } while (leftDelay > 0);
            if (isBeforeAnimation)
                owner.animator.SetBool("isBeforeAttack", false);

            OnBeforeAttackEnd();

            Attack();
            beforeAttackCorou = null;
            afterAttackCorou = AfterAttack();
            StartCoroutine(afterAttackCorou);
        }

        IEnumerator AfterAttack()
        {
            float leftDelay = afterDelay;
            if (isAfterAnimation)
                owner.animator.SetBool("isAfterAttack", true);

            OnAfterAttackStart();

            do
            {
                yield return null;
                leftDelay -= Time.deltaTime;
            } while (leftDelay > 0);

            if (isAfterAnimation)
                owner.animator.SetBool("isAfterAttack", false);

            OnAfterAttackEnd();

            afterAttackCorou = null;
        }

        private void OnEnable()
        {
            AnimatorControllerParameter[] parameters = owner.animator.parameters;

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
            if (owner.targetable.target && _weapon.attackDistance == 0 || targetDistance <= _weapon.attackDistance)
            {
                if(beforeAttackCorou == null && afterAttackCorou == null)
                {
                    beforeAttackCorou = BeforeAttack();
                    StartCoroutine(beforeAttackCorou);
                }
            }
        }

        protected abstract void OnBeforeAttackStart();
        protected abstract void OnBeforeAttackEnd();
        protected abstract void OnAfterAttackStart();
        protected abstract void OnAfterAttackEnd();
    }
}
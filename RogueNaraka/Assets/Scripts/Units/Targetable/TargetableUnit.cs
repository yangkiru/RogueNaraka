using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts.Targetable
{
    public abstract class TargetableUnit : MonoBehaviour
    {
        [SerializeField]
        Unit owner;
        public Unit target { get { return _target; } }
        [SerializeField]
        Unit _target;

        public float targetDistance { get { return _targetDistance; } }
        float _targetDistance;

        float delay;
        float leftDelay;

        public Vector2 direction { get { if (target) return target.transform.position - transform.position; else return Vector2.zero; } }

        public bool IsTargetable { get { return isTargetable; } set { isTargetable = value; } }
        bool isTargetable = true;

        void Reset()
        {
            owner = GetComponent<Unit>();
        }

        void Update()
        {
            if (leftDelay > 0)
            {
                leftDelay -= Time.deltaTime;
                return;
            }
            else
                leftDelay = 0.1f;
            _target = GetTarget();
            _targetDistance = Distance(_target);
        }

        protected float Distance(Unit target)
        {
            if (target == null || target.deathable.isDeath)
                return float.PositiveInfinity;
            else
                return Vector2.SqrMagnitude(target.cachedTransform.position - owner.cachedTransform.position);
        }

        abstract protected Unit GetTarget();

        private void OnDisable()
        {
            _target = null;
            isTargetable = true;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts.Targetable
{
    public abstract class TargetableUnit : MonoBehaviour
    {
        Unit owner;
        public Unit target { get { return _target; } }
        Unit _target;

        float delay;
        float leftDelay;
        bool isDeath;

        public Vector2 direction { get { return target.transform.position - transform.position; } }

        void Awake()
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
            else if (leftDelay < 0)
                leftDelay = 0;
            _target = GetTarget();
        }

        protected float Distance(Unit target)
        {
            return Vector2.SqrMagnitude(target.transform.position - transform.position);
        }

        abstract protected Unit GetTarget();
    }
}
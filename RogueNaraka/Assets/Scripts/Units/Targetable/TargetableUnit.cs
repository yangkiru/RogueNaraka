﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts.Targetable
{
    public abstract class TargetableUnit : MonoBehaviour
    {
        Unit owner;
        public Unit target { get { return _target; } }
        [SerializeField]
        Unit _target;

        public float targetDistance { get { return _targetDistance; } }
        float _targetDistance;

        float delay;
        float leftDelay;
        bool isDeath;

        public Vector2 direction { get { if (target) return target.transform.position - transform.position; else return Vector2.zero; } }

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
            _targetDistance = Distance(target);
        }

        protected float Distance(Unit target)
        {
            if (!target)
                return float.PositiveInfinity;
            return Vector2.SqrMagnitude(target.transform.position - transform.position);
        }

        abstract protected Unit GetTarget();
    }
}
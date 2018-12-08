using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts.Targetable
{
    public abstract class TargetableUnit : MonoBehaviour
    {
        Unit owner;
        Unit target;

        float delay;
        float leftDelay;

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
            target = GetTarget();
        }

        protected int Compare(Unit a, Unit b)
        {
            float aDistance = Vector2.SqrMagnitude(a.transform.position - transform.position);
            float bDistance = Vector2.SqrMagnitude(b.transform.position - transform.position);

            return bDistance.CompareTo(aDistance);
        }

        abstract protected Unit GetTarget();
    }
}
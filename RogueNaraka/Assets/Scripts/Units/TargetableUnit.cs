using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts
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

        abstract protected Unit GetTarget();
    }
}
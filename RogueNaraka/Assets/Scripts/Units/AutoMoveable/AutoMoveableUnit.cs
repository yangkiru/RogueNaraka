using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts.AutoMoveable
{
    public abstract class AutoMoveableUnit : MonoBehaviour
    {
        [SerializeField]
        protected Unit unit;
        [SerializeField]
        protected MoveableUnit moveable;

        protected float distance;

        [SerializeField]
        float delay;
        [SerializeField]
        float leftDelay;

        void Reset()
        {
            unit = GetComponent<Unit>();
            moveable = GetComponent<MoveableUnit>();
        }

        public virtual void Init(UnitData data)
        {
            distance = data.moveDistance;
            delay = data.moveDelay;
            leftDelay = 0;
        }

        void Update()
        {
            if (moveable.agent.velocity.x >= 0.1f || moveable.agent.velocity.y >= 0.1f)
                return;
            if (leftDelay > 0)
            {
                leftDelay -= Time.deltaTime;
                return;
            }
            else if (leftDelay <= 0)
                leftDelay = delay;
            AutoMove();
        }

        protected abstract void AutoMove();
    }
}

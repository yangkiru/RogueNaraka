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

        float delay;
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
            if (moveable.agent.velocity != Vector2.zero)
                return;
            if (leftDelay > 0)
            {
                leftDelay -= Time.deltaTime;
                return;
            }
            else if (leftDelay < 0)
                leftDelay = 0;
            AutoMove();
        }

        protected abstract void AutoMove();
    }
}

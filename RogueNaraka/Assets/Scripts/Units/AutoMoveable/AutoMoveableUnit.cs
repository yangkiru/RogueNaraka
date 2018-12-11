using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts.AutoMoveable
{
    public abstract class AutoMoveableUnit : MonoBehaviour
    {
        protected MoveableUnit moveable;

        protected float distance;

        float delay;
        float leftDelay;

        void Awake()
        {
            moveable = GetComponent<MoveableUnit>();
        }

        public virtual void Init(UnitData data)
        {
            distance = data.moveDistance;
            delay = data.moveDelay;
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
            if (moveable.agent.velocity != Vector2.zero)
                return;
            AutoMove();
        }

        protected abstract void AutoMove();
    }
}

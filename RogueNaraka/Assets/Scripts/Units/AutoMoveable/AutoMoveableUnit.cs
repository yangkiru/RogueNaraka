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

        protected Transform cashedTransform;
        protected float distance;

        [SerializeField]
        float delay;
        
        public float leftDelay;

        void Reset()
        {
            unit = GetComponent<Unit>();
            moveable = GetComponent<MoveableUnit>();
        }

        private void Awake()
        {
            cashedTransform = transform;
        }

        public virtual void Init(UnitData data)
        {
            distance = data.moveDistance;
            delay = data.moveDelay;
            leftDelay = 0;
        }

        void Update()
        {
            /*
            if (moveable.agent.velocity.x >= 0.1f || moveable.agent.velocity.y >= 0.1f)
                return;
            */
            if (leftDelay > 0)
            {
                leftDelay -= Mathf.Max(0, Time.deltaTime * (1 + unit.stat.GetCurrent(STAT.SPD) * 0.1f));
                return;
            }
            else if (leftDelay <= 0)
                leftDelay = delay;
            AutoMove();
        }

        protected abstract void AutoMove();
    }
}

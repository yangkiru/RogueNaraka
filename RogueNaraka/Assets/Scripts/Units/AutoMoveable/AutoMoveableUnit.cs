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
        
        public float leftDelay;

        void Reset()
        {
            unit = GetComponent<Unit>();
            moveable = GetComponent<MoveableUnit>();
        }

        public virtual void Init(UnitData data)
        {
            distance = data.moveDistance;
            delay = data.moveDelay;
            leftDelay = Random.Range(delay * 0.5f, delay);
        }

        void FixedUpdate()
        {
            if (moveable.CurSpeed > 0 || unit.isStun)
                return;
            
            if (leftDelay > 0)
            {
                float amount = Time.fixedDeltaTime * (1 + unit.stat.GetCurrent(STAT.SPD) * 0.1f);
                leftDelay -= amount > 0 ? amount : 0;
                return;
            }
            else if (leftDelay <= 0)
                leftDelay = delay;
            AutoMove();
        }

        protected abstract void AutoMove();
    }
}

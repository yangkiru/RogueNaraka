using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNav;

namespace RogueNaraka.UnitScripts
{
    public class MoveableUnit : MonoBehaviour
    {
        [SerializeField]
        Unit unit;

        private Vector2 destination;
        private Action onArrivedCallback;
        public Action OnArrivedCallback { get { return this.onArrivedCallback; } }

        public float speed {
            get {
                return unitSpeed * (1 + unit.stat.GetCurrent(STAT.SPD) * 0.1f) *
                    (1 + factor);
            }
        }
        float unitSpeed;
        public float factor;

        bool isWalk;
        bool _isWalk;

        void Reset()
        {
            unit = GetComponent<Unit>();
        }

        public void Init(UnitData data)
        {
            SetSpeed(data.moveSpeed);
            //agent.enabled = true;
            //agent.Stop();
        }

        public void SetSpeed(float speed)
        {
            unitSpeed = speed;
        }
        
        /// <summary>목적지를 설정합니다.</summary>
        public void SetDestination(Vector3 pos, System.Action callback = null)
        {
            this.destination = pos;
            this.onArrivedCallback = callback;
            /*
            if(!unit.isStun)
                _agent.SetDestination(pos, callback);
            */
        }

        void Update()
        {
            //애니메이션 관련
            agent.maxSpeed = speed;
            float x = 0, y = 0;
            if (_agent.velocity.x != 0 || _agent.velocity.y != 0)
            {
                x = _agent.velocity.x;
                y = _agent.velocity.y;
                isWalk = true;
            }
            else
                isWalk = false;
            
            if (unit.targetable.target)
            {
                x = unit.targetable.direction.x;
                y = unit.targetable.direction.y;
            }

            unit.animator.SetFloat("x", x);
            unit.animator.SetFloat("y", y);
            
            if (isWalk != _isWalk)
                unit.animator.SetBool("isWalk", isWalk);
            _isWalk = isWalk;
            //
        }
    }
}

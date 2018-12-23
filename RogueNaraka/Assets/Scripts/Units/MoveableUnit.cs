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
        public PolyNavAgent agent
        { get { return _agent; } }
        [SerializeField]
        PolyNavAgent _agent;

        public float speed { get { return unitSpeed * (1 + unit.stat.GetCurrent(STAT.SPD) * 0.1f); } }
        float unitSpeed;

        bool isWalk;
        bool _isWalk;

        void Reset()
        {
            unit = GetComponent<Unit>();
            _agent = GetComponent<PolyNavAgent>();
        }

        public void Init(UnitData data)
        {
            SetSpeed(data.moveSpeed);
            agent.enabled = true;
        }

        public void SetSpeed(float speed)
        {
            unitSpeed = speed;
        }

        public void Move(Vector3 pos, Action<bool> callback = null)
        {
            _agent.SetDestination(pos, callback);
        }

        void Update()
        {
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
        }
    }
}

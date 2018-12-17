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

        public float speed { get { return unitSpeed; } }
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
        }

        public void SetSpeed(float speed)
        {
            unitSpeed = speed;
            agent.maxSpeed = speed;
        }

        public void Move(Vector3 pos, Action<bool> callback = null)
        {
            _agent.SetDestination(pos, callback);
        }

        public void StopMove()
        {
            _agent.Stop();
        }

        void Update()
        {
            if (_agent.velocity.x >= 0.01f || _agent.velocity.y >= 0.01f)
            {
                unit.animator.SetFloat("x", _agent.velocity.x);
                unit.animator.SetFloat("y", _agent.velocity.y);
                isWalk = true;
            }
            else
                isWalk = false;
            if (isWalk != _isWalk)
                unit.animator.SetBool("isWalk", isWalk);
            _isWalk = isWalk;
        }
    }
}

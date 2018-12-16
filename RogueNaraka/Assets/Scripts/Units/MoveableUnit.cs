using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNav;

namespace RogueNaraka.UnitScripts
{
    public class MoveableUnit : MonoBehaviour
    {
        Unit unit;
        public PolyNavAgent agent
        { get { return _agent; } }
        [SerializeField]
        PolyNavAgent _agent;

        public float speed { get { return unitSpeed; } }
        float unitSpeed;

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
    }
}

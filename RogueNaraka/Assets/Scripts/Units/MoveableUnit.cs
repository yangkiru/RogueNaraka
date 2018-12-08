using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNav;

namespace RogueNaraka.UnitScripts
{
    public class MoveableUnit : MonoBehaviour
    {
        public PolyNavAgent agent
        { get { return _agent; } }
        PolyNavAgent _agent;

        public float speed { get { return unitSpeed; } }
        float unitSpeed;

        void Awake()
        {
            _agent = GetComponent<PolyNavAgent>();
        }

        public void Init(UnitData data)
        {
            unitSpeed = data.moveSpeed;
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

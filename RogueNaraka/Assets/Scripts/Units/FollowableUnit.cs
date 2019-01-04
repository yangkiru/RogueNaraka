﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts
{
    public class FollowableUnit : MonoBehaviour
    {
        public List<Unit> followers { get { return _followers; } }
        [SerializeField]
        List<Unit> _followers = new List<Unit>();

        public float distance { get { return _distance; } }
        [SerializeField]
        float _distance;

        [SerializeField]
        Unit unit;

        private void Reset()
        {
            unit = GetComponent<Unit>();
        }

        public void Init(UnitData data)
        {
            _distance = data.followDistance;
        }

        public void AddFollower(Unit unit)
        {
            _followers.Add(unit);
            unit.followMoveable.SetTarget(this.unit);
        }

        public void RemoveFollower(Unit unit)
        {
            if (_followers.Remove(unit))
                unit.followMoveable.LostTarget();
        }

        private void OnDisable()
        {
            for(int i = _followers.Count - 1; i >= 0; i--)
            {
                RemoveFollower(_followers[i]);
            }
        }
    }
}
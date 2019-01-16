using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts
{
    public class FollowableUnit : MonoBehaviour
    {
        public List<Unit> followers { get { return _followers; } }
        [SerializeField]
        List<Unit> _followers = new List<Unit>();

        [SerializeField]
        Unit unit;

        private void Reset()
        {
            unit = GetComponent<Unit>();
        }

        public void Init(UnitData data)
        {
            _followers.Clear();
        }

        public void AddFollower(Unit unit)
        {
            Debug.Log("AddFollower" + this.unit.name + " " + unit.name);
            _followers.Add(unit);
            unit.followMoveable.SetTarget(this.unit);
        }

        public void RemoveFollower(Unit unit)
        {
            if (_followers.Remove(unit))
                unit.followMoveable.LostTarget();
        }

        public void OnDeath()
        {
            for(int i = _followers.Count - 1; i >= 0; i--)
            {
                RemoveFollower(_followers[i]);
            }
        }
    }
}
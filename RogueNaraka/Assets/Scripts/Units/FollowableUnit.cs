using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts
{
    public class FollowableUnit : MonoBehaviour
    {
        public LinkedList<Unit> Followers { get { return followers; } }
        [SerializeField]
        LinkedList<Unit> followers = new LinkedList<Unit>();

        [SerializeField]
        Unit unit;

        public bool isFollow = true;

        private void Reset()
        {
            unit = GetComponent<Unit>();
        }

        public void Init(UnitData data)
        {
            followers.Clear();
        }

        public void OnDisable()
        {
            OnDeath();
        }

        public void AddFollower(Unit unit)
        {
            Debug.Log("AddFollower" + this.unit.name + " " + unit.name);
            unit.followMoveable.Init(followers.AddLast(unit), this.unit);
        }

        public void RemoveFollower(Unit unit)
        {
            followers.Remove(unit.followMoveable.Node);
        }

        public void OnDeath()
        {
            for(int i = followers.Count - 1; i >= 0; i--)
            {
                followers.Last.Value.followMoveable.OnLostTarget();
                followers.RemoveLast();
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.BulletScripts
{
    public class OwnerableBullet : MonoBehaviour
    {
        public Unit unit { get { return _unit; } }
        [SerializeField]
        Unit _unit;
        public int layer
        {
            get
            {
                if (unit) return unit.gameObject.layer;
                else return -1;
            }
        }
        
        public void SetOwner(Unit owner)
        {
            _unit = owner;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.BulletScripts
{
    public class OwnerableBullet : MonoBehaviour
    {
        public Unit owner { get { return _owner; } }
        [SerializeField]
        Unit _owner;
        public int layer { get { return owner.gameObject.layer; } }
        
        public void SetOwner(Unit owner)
        {
            _owner = owner;
        }
    }
}

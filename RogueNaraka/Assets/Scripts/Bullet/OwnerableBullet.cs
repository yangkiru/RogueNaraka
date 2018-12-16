using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.BulletScripts
{
    public class OwnerableBullet : MonoBehaviour
    {
        [SerializeField]
        Unit owner;
        public int layer { get { return owner.gameObject.layer; } }
        
        public void SetOwner(Unit owner)
        {
            this.owner = owner;
        }
    }
}

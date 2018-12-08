using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.UnitScripts;

namespace RogueNaraka.BulletScripts
{
    public class OwnerableBullet : MonoBehaviour
    {
        [SerializeField]
        OldUnit owner;
        public LayerMask layerMask { get { return owner.gameObject.layer; } }
        
        public void SetOwner(OldUnit owner)
        {
            this.owner = owner;
        }
    }
}

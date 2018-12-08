using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RogueNaraka.Bullet
{
    public class OwnerableBullet : MonoBehaviour
    {
        [SerializeField]
        Unit owner;
        public LayerMask layerMask
        { get { return owner.gameObject.layer; } }
        
        public void SetOwner(Unit owner)
        {
            this.owner = owner;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.BulletScripts;

namespace RogueNaraka.UnitScripts
{
    public class Unit : MonoBehaviour
    {
        protected MoveableUnit moveable;
        protected AttackableUnit attackable;

        void Awake()
        {
            moveable = GetComponent<MoveableUnit>();
            attackable = GetComponent<AttackableUnit>();
        }
    }
}
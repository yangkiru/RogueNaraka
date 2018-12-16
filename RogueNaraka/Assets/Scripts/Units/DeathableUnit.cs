using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts
{
    public class DeathableUnit : MonoBehaviour
    {
        Unit unit;

        public bool isDeath { get { return _isDeath; } }
        bool _isDeath;

        void Reset()
        {
            unit = GetComponent<Unit>();
        }

        public void Init()
        {
            _isDeath = false;
        }

        public void Death()
        {
            _isDeath = true;
            unit.animator.SetBool("isDeath", true);
        }

        public void Revive()
        {
            _isDeath = false;
            unit.animator.SetBool("isDeath", false);
        }
    }
}

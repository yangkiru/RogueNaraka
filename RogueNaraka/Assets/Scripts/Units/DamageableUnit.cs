using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts
{
    public class DamageableUnit : MonoBehaviour
    {
        Unit unit;
        HpableUnit hpable;
        private void Awake()
        {
            unit = GetComponent<Unit>();
            hpable = unit.hpable;
        }

        public void Damage(float amount)
        {
            hpable.AddHp(amount);
        }
    }
}

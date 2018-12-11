using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts
{
    public class DamageableUnit : MonoBehaviour
    {
        HpableUnit hpable;
        private void Awake()
        {
            hpable = GetComponent<HpableUnit>();
        }

        public void Damage(float amount)
        {
            hpable.AddHp(amount);
        }
    }
}

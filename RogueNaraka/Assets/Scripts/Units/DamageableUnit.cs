using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts
{
    public class DamageableUnit : MonoBehaviour
    {
        [SerializeField]
        HpableUnit hpable;
        private void Reset()
        {
            hpable = GetComponent<HpableUnit>();
        }

        public void Damage(float amount)
        {
            hpable.AddHp(amount);
        }
    }
}

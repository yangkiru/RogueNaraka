using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts
{
    public class HpableUnit : MonoBehaviour
    {
        public float currentHp { get { return _currentHp; } }
        float _currentHp;
        public float maxHp { get { return stat.hp; } }

        public float regenTime { get { return 1; } }
        float currentTime;//Current Regen Time

        public float regenHp { get { return stat.hpRegen; } }

        Stat stat;

        public void Init(Stat stat)
        {
            this.stat = stat;
            currentTime = 0;
        }

        public void SetHp(float value)
        {
            if (value > maxHp)
                _currentHp = maxHp;
            else if (value < 0)
                _currentHp = 0;
            else
                _currentHp = value;
        }

        public void AddHp(float amount)
        {
            float result = _currentHp + amount;

            if (amount > 0 && result > maxHp)
                result = maxHp;
            else if (result < 0)
                result = 0;
            _currentHp = result;
        }

        void Regen()
        {
            currentTime += Time.deltaTime;
            if(currentTime >= regenTime)
            {
                AddHp(regenHp);
                currentTime = 0;
            }
        }

        private void Update()
        {
            Regen();
        }

    }
}

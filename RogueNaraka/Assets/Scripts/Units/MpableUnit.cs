﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts
{
    public class MpableUnit : MonoBehaviour
    {

        public float currentMp { get { return _currentMp; } }
        float _currentMp;
        public float maxMp { get { return stat.mp; } }

        public float regenTime { get { return 1; } }
        float currentTime;

        public float regenMp { get { return stat.mpRegen; } }

        Stat stat;

        public void Init(Stat stat)
        {
            this.stat = stat;
            currentTime = 0;
        }

        public void SetMp(float value)
        {
            if (value > maxMp)
                _currentMp = maxMp;
            else if (value < 0)
                _currentMp = 0;
            else
                _currentMp = value;
        }

        public void AddMp(float amount)
        {
            float result = _currentMp + amount;

            if (amount > 0 && result > maxMp)
                result = maxMp;
            else if (result < 0)
                result = 0;
            _currentMp = result;
        }

        void Regen()
        {
            currentTime += Time.deltaTime;
            if (currentTime >= regenTime)
            {
                AddMp(regenMp);
                currentTime = 0;
            }
        }

        private void Update()
        {
            Regen();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts
{
    public class MpableUnit : MonoBehaviour
    {
        [SerializeField]
        Unit unit;

        public float currentMp { get { return _currentMp; } }
        [SerializeField]
        float _currentMp;
        public float maxMp { get { return stat.mp; } }

        public float regenTime { get { return 1; } }
        float currentTime;

        public float regenMp { get { return stat.mpRegen * 0.1f; } }

        Stat stat;

        void Reset()
        {
            unit = GetComponent<Unit>();
        }

        public void Init(Stat stat)
        {
            this.stat = stat;
            _currentMp = stat.currentMp;
            currentTime = 0;
        }

        public void SetMp(float value)
        {
            if (value > maxMp)
                _currentMp = maxMp;
            else if (value >= 0)
                _currentMp = value;

            else
                _currentMp = 0;
            stat.currentMp = _currentMp;
        }

        public void SetFullMp(bool isTxt = false)
        {
            if (isTxt)
                Heal(maxMp - _currentMp);
            else
                _currentMp = maxMp;
        }

        public void AddMp(float amount)
        {
            float result = _currentMp + amount;

            if (amount > 0 && result > maxMp)
                result = maxMp;
            else if (result <= 0)
                result = 0;

            _currentMp = result;
            stat.currentMp = _currentMp;
        }

        public void Heal(float amount)
        {
            float result = Mathf.Min(maxMp, _currentMp + amount);

            float heal = result - _currentMp;

            if (heal > 0)
                PointTxtManager.instance.TxtOnHead(heal, transform, Color.blue);
            _currentMp = result;
            stat.currentMp = _currentMp;
        }

        void Regen()
        {
            if (unit.deathable.isDeath)
                return;
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
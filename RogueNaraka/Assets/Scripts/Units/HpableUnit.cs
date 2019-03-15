using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts
{
    public class HpableUnit : MonoBehaviour
    {
        [SerializeField]
        Unit unit;

        public float currentHp { get { return _currentHp; } }
        [SerializeField]
        float _currentHp;
        public float maxHp { get { return stat.hp; } }

        public float regenTime { get { return 1; } }
        float currentTime;//Current Regen Time

        public float regenHp { get { return stat.hpRegen * 0.1f; } }
        
        public float regenSpeed = 1;
        public bool isInvincible;

        Stat stat;

        void Reset()
        {
            unit = GetComponent<Unit>();
        }

        public void Init(Stat stat)
        {
            this.stat = stat;
            _currentHp = stat.currentHp;
            currentTime = 0;
        }

        public void SetHp(float value)
        {
            if (value > maxHp)
                _currentHp = maxHp;
            else if (value >= 0)
                _currentHp = value;
            else
                _currentHp = 0;
            stat.currentHp = _currentHp;
            if(_currentHp <= 0 && !unit.deathable.isDeath)
                unit.deathable.Death();
        }

        public void SetFullHp(bool isTxt = false)
        {
            if (isTxt)
                Heal(maxHp - _currentHp);
            else
                _currentHp = maxHp;
        }

        public void AddHp(float amount)
        {
            float result = _currentHp + amount;

            if (amount > 0 && result > maxHp)
                result = maxHp;
            else if (result <= 0)
                result = 0;

            _currentHp = result;
            stat.currentHp = _currentHp;

            if (result <= 0 && !unit.deathable.isDeath)
            {
                unit.deathable.Death();
            }
        }

        public void Heal(float amount)
        {
            float result = Mathf.Min(maxHp, _currentHp + amount);

            float heal = result - _currentHp;

            if(heal > 0)
                PointTxtManager.instance.TxtOnHead(heal, transform, Color.green);
            _currentHp = result;
            stat.currentHp = _currentHp;
        }

        //void Regen()
        //{
        //    if (unit.deathable.isDeath || stat == null)
        //        return;
        //    currentTime += Time.deltaTime;
        //    if(currentTime >= regenTime)
        //    {
        //        AddHp(regenHp);
        //        currentTime = 0;
        //    }
        //}

        //private void Update()
        //{
        //    Regen();
        //}

        IEnumerator RegenCorou()
        {
            while (true)
            {
                float t = regenTime;
                do
                {
                    yield return null;
                    t -= Time.deltaTime * regenSpeed;
                } while (t > 0);
                if(!unit.deathable.isDeath && _currentHp > 0)
                    AddHp(regenHp);
            }
        }

        private void OnEnable()
        {
            StartCoroutine(RegenCorou());
        }

    }
}

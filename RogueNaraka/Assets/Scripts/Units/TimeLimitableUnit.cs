using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts
{
    public class TimeLimitableUnit : MonoBehaviour
    {
        [SerializeField]
        Unit unit;
        public float time { get { return _time; } }
        [SerializeField]
        float _time;
        [SerializeField]
        float leftTime;

        private void Reset()
        {
            unit = GetComponent<Unit>();
        }

        public void Init(float limitTime)
        {
            _time = limitTime;
            leftTime = _time;
        }

        private void Update()
        {
            if (leftTime > 0)
                leftTime -= Time.deltaTime;
            else if (_time != 0)
            {
                unit.Kill(false);
                enabled = false;
            }
        }
    }
}
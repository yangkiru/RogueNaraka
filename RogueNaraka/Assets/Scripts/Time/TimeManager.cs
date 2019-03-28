using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.SingletonPattern;

namespace RogueNaraka.TimeScripts {
    public class TimeManager : MonoSingleton<TimeManager> {
        const float MAX_DELTA_TIME = 0.5f;

        /// <summary> 유니티 deltaTime을 리턴합니다. MAX_DELTA_TIME보다 큰 경우 MAX_DELTA_TIME을 리턴합니다. </summary>
        public float DeltaTime { 
            get { 
                if(Time.deltaTime > MAX_DELTA_TIME) {
                    return MAX_DELTA_TIME;
                } else {
                    return Time.deltaTime;
                }
            }
        }

        /// <summary> 유니티 fixedDeltaTime을 리턴합니다. </summary>
        public float FixedDeltaTime { get { return Time.fixedDeltaTime; } }
        
        void Update() {
            
        }

        /// <summary> 게임 종료시 반드시 해당 함수를 실행해주세요. </summary>
        public override void OnDestroy() {
            base.OnDestroy();
        }
    }
}
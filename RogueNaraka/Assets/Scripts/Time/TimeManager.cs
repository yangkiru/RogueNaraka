﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.SingletonPattern;

namespace RogueNaraka.TimeScripts {
    public class TimeManager : MonoSingleton<TimeManager> {
        /// <summary> 해당 클래스 사용 종료 시 반드시 해당 함수를 실행해주세요. </summary>
        public override void OnDestroy() {
            base.OnDestroy();
        }

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

        public bool CheckDateTimeInEventTime(DateTime _check, DateTime _start, DateTime _end) {
            if(_check.CompareTo(_start) < 0) {
                return false;
            } else if(_check.CompareTo(_end) > 0) {
                return false;
            }
            return true;
        }
    }
}
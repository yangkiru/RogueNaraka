using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;
using RogueNaraka.TimeScripts;

namespace RogueNaraka.TheBackendScripts {
    public enum PUSH_EVENT_TYPE {
        SOUL_REWARD,
        END
    }
    public abstract class PushEvent {
        protected PUSH_EVENT_TYPE type;
        protected int pushEventId;
        protected bool isRewarded;
        protected DateTime startDateTime;
        protected DateTime endDateTime;

        public abstract void Initialize(int _id, bool _isRewarded, DateTime _startdateTime, DateTime _endDateTime, Dictionary<string, int> _rewardInfoDic);
        public virtual bool CheckAcceptable(DateTime _now) {
            if(isRewarded) {
                return false;
            }
            if(TimeManager.Instance.CheckDateTimeInEventTime(_now, this.startDateTime, this.endDateTime)) {
                return true;
            }
            return false;
        }
        public abstract Param AcceptReward(DateTime _now);
        public abstract void PrintInfo();
    }
}
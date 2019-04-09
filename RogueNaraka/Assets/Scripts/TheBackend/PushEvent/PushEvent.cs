using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.TheBackendScripts {
    public enum PUSH_EVENT_TYPE {
        SOUL_REWARD,
        END
    }
    public abstract class PushEvent {
        protected PUSH_EVENT_TYPE type;
        protected int pushEventId;
        protected bool isAccepted;
        protected DateTime startdateTime;
        protected DateTime endDateTime;

        public abstract void Initialize(bool _isRewarded, Dictionary<string, int> _rewardInfoDic);
        public abstract bool CheckAcceptable();
        public abstract void AcceptReward();
    }
}
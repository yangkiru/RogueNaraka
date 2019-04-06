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
        private PUSH_EVENT_TYPE type;
        private int pushEventId;
        private bool isAccepted;
        private DateTime startdateTime;
        private DateTime endDateTime;

        public abstract bool CheckAcceptable();
        public abstract void AcceptReward();
    }
}
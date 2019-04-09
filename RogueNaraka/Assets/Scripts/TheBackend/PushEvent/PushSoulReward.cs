using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.TheBackendScripts {
    public class PushSoulReward : PushEvent {
        private int rewardSoulAmounts;

        public override void Initialize(bool _isAccepted, Dictionary<string, int> _rewardInfoDic) {
            this.type = PUSH_EVENT_TYPE.SOUL_REWARD;
            this.isAccepted = _isAccepted;
            this.rewardSoulAmounts = _rewardInfoDic["soulAmounts"];
        }

        public override bool CheckAcceptable() {
            return false;
        }

        public override void AcceptReward() {
            Debug.LogError(string.Format("Reward Soul! : {0} souls"));
        }
    }
}

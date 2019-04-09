using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;

namespace RogueNaraka.TheBackendScripts {
    public class PushSoulReward : PushEvent {
        private int rewardSoulAmounts;

        public override void Initialize(int _id, bool _isRewarded, DateTime _startdateTime, DateTime _endDateTime, Dictionary<string, int> _rewardInfoDic) {
            this.type = PUSH_EVENT_TYPE.SOUL_REWARD;
            this.pushEventId = _id;
            this.isRewarded = _isRewarded;
            this.startDateTime = _startdateTime;
            this.endDateTime = _endDateTime;
            this.rewardSoulAmounts = _rewardInfoDic["SoulAmounts"];
        }

        public override void AcceptReward(DateTime _now) {
            this.isRewarded = true;
            Param rewardParam = new Param();
            rewardParam.Add(string.Format("Id_{0}", this.pushEventId), _now.ToString("yyyy-MM-dd HH:mm:ss"));
            TheBackendManager.Instance.SendParamToRewardedPushInfo(rewardParam);
            Debug.LogError(string.Format("Reward Soul! : {0} souls", this.rewardSoulAmounts));
        }

        public override void PrintInfo() {
            Debug.LogError(string.Format("PushSoulReward Info : Id : {0}, IsRewarded : {1}, StartTime : {2}, EndTime : {3}, Type : {4}, RewardAmount : {5}", 
                this.pushEventId, this.isRewarded, this.startDateTime, this.endDateTime, this.type, this.rewardSoulAmounts));
        }
    }
}

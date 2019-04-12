using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;
using RogueNaraka.PopUpScripts;

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

        public override Param AcceptReward(DateTime _now) {
            this.isRewarded = true;
            Param rewardParam = new Param();
            rewardParam.Add(string.Format("Id_{0}", this.pushEventId), _now.ToString("yyyy-MM-dd HH:mm:ss"));
            MoneyManager.instance.AddSoul(this.rewardSoulAmounts, true);
            string pushRewardContext = "";
            switch(GameManager.language) {
                case Language.English:
                    pushRewardContext = "You have acquired {0} Souls as a reward.";
                break;
                case Language.Korean:
                    pushRewardContext = "보상으로 {0} 소울을 획득하였습니다.";
                break;
            } 
            PopUpManager.Instance.ActivateOneButtonPopUp(
                string.Format(pushRewardContext, this.rewardSoulAmounts),
                (OneButtonPopUpController _popUp) => { 
                    _popUp.DeactivatePopUp(); 
                    PopUpManager.Instance.DeactivateBackPanel();
                    GameManager.instance.SetPause(false);
                });
            Debug.Log(string.Format("Reward Soul! : {0} souls", this.rewardSoulAmounts));
            return rewardParam;
        }

        public override void PrintInfo() {
            Debug.LogError(string.Format("PushSoulReward Info : Id : {0}, IsRewarded : {1}, StartTime : {2}, EndTime : {3}, Type : {4}, RewardAmount : {5}", 
                this.pushEventId, this.isRewarded, this.startDateTime, this.endDateTime, this.type, this.rewardSoulAmounts));
        }
    }
}

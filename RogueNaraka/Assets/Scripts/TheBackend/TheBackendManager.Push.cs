using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using RogueNaraka.SingletonPattern;

namespace RogueNaraka.TheBackendScripts {
    public partial class TheBackendManager : MonoSingleton<TheBackendManager> {
        private const int MAX_NUM_PUSH_REWARD_TYPE = 10;
        private Dictionary<string, PushEvent> RewardedPushDictionary = new Dictionary<string, PushEvent>();

        private string inDateForPushReward;

        private IEnumerator LoadRewardedPushInfo() {
            yield return new WaitUntil(() => this.isLoginSuccess);

            Backend.GameInfo.GetPrivateContents("PushReward", (callback) => {
                var pushList = callback.GetReturnValuetoJSON()["rows"];
                if(pushList.Count == 0) {
                    Param newPushRewardParam = new Param();
                    Backend.GameInfo.Insert("PushReward", newPushRewardParam, (insertCallback) => {
                        Backend.GameInfo.GetPrivateContents("PushReward", (reCallback) => {
                            this.inDateForPushReward = reCallback.GetReturnValuetoJSON()["rows"][0]["inDate"]["S"].ToString();
                        });
                    });
                } else {
                    this.inDateForPushReward = pushList[0]["inDate"]["S"].ToString();
                    IDictionary pushDictionaryFromJson = pushList[0] as IDictionary;
                }
            });
        }

        private IEnumerator LoadPushRewardInfo() {
            yield return null;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackEnd;
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using LitJson;
using RogueNaraka.SingletonPattern;
using RogueNaraka.TimeScripts;

namespace RogueNaraka.TheBackendScripts {
    public partial class TheBackendManager : MonoSingleton<TheBackendManager> {
        private const int MAX_NUM_PUSH_REWARD_TYPE = 15;

        private Dictionary<string, DateTime> rewardedPushDictionary = new Dictionary<string, DateTime>();
        private Dictionary<string, PushEvent> pushRewardDictionary = new Dictionary<string, PushEvent>();

        private enum PROCESS_GET_CHARTFILENAME {NOT_GET, GETTING, GOT}
        private PROCESS_GET_CHARTFILENAME getFilenameProcess = PROCESS_GET_CHARTFILENAME.NOT_GET;

        private string inDateForPushReward;
        private string pushRewardChartFileName;
        private bool isGetPushRewardChartFileName;
        private bool isLoadedRewardedPushInfo;

        private Coroutine getChartAsyncCoroutine;
        private Coroutine checkRewardPushesCoroutine;

        //릴리즈할때 반드시 해당 변수 false로 바꿔주세요!!!!!!
        private bool isDevelop = false;

        private void StartForPush() {
            StartCoroutine(LoadRewardedPushInfo());
            this.getChartAsyncCoroutine = StartCoroutine(GetChartAsync());
        }

        private void StopForPush() {
            StopCoroutine(this.getChartAsyncCoroutine);
            this.getChartAsyncCoroutine = null;
        }

        public void PrintAllPushRewardInfoes() {
            var enumerator = this.pushRewardDictionary.GetEnumerator();
            while(enumerator.MoveNext()) {
                enumerator.Current.Value.PrintInfo();
            }
        }

        private void GetCahrtFimeName() {
            this.getFilenameProcess = PROCESS_GET_CHARTFILENAME.GETTING;
            Backend.Chart.GetChartList((callback) => {
                if(callback.IsSuccess()) {
                    var chartJson = callback.GetReturnValuetoJSON()["rows"];
                    for(int i = 0; i < chartJson.Count; ++i) {
                        if(chartJson[i]["chartName"]["S"].ToString() == "PushRewardChart") {
                            this.pushRewardChartFileName = chartJson[i]["selectedChartFileId"]["N"].ToString();
                            break;
                        }
                    }
                    this.getFilenameProcess = PROCESS_GET_CHARTFILENAME.GOT;
                } else {
                    this.getFilenameProcess = PROCESS_GET_CHARTFILENAME.NOT_GET;
                }
            });
        }

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
                    JsonData pushJson = pushList[0] as JsonData;
                    for(int i = 0; i < MAX_NUM_PUSH_REWARD_TYPE; ++i) {
                        string pushId = string.Format("Id_{0}", i);
                        if(pushJson.Keys.Contains(pushId)) {
                            this.rewardedPushDictionary.Add(pushId, DateTime.Parse(pushJson[pushId]["S"].ToString()));
                        }
                    }
                }
                this.isLoadedRewardedPushInfo = true;
            });
        }

        private IEnumerator GetChartAsync() {
            yield return new WaitUntil(() => this.isLoginSuccess);

            while(true) {
                switch(this.getFilenameProcess) {
                    case PROCESS_GET_CHARTFILENAME.NOT_GET:
                        GetCahrtFimeName();
                        yield return this.waitForOneSeconds;
                    break;
                    case PROCESS_GET_CHARTFILENAME.GETTING:
                        yield return this.waitForOneSeconds;
                    break;
                    case PROCESS_GET_CHARTFILENAME.GOT:
                        Backend.Chart.GetChartContents(this.pushRewardChartFileName, (chartContentscallback) => {
                        if(chartContentscallback.IsSuccess()) {
                                StartCoroutine(SavePushRewardInfo(chartContentscallback.GetReturnValuetoJSON()["rows"]));
                            } else {
                                this.getFilenameProcess = PROCESS_GET_CHARTFILENAME.NOT_GET;
                            }
                        });
                        if(this.getFilenameProcess == PROCESS_GET_CHARTFILENAME.GOT) {
                            yield return this.waitForThirtySeconds;
                        } else {
                            yield return this.waitForTenSeconds;
                        }
                    break;
                }
            }
        }

        private IEnumerator SavePushRewardInfo(JsonData _pushRewardChart) {
            yield return new WaitUntil(() => this.isLoadedRewardedPushInfo);

            for(int i = 0; i < _pushRewardChart.Count; ++i) {
                string pushEventTypeStr = _pushRewardChart[i]["rewardType"]["S"].ToString();
                if(pushEventTypeStr == "") {
                    continue;
                }
                string pushEventId = string.Format("Id_{0}", _pushRewardChart[i]["num"]["S"].ToString());
                if(this.pushRewardDictionary.ContainsKey(pushEventId)) {
                    continue;
                }
                bool isDevelopingPush = bool.Parse(_pushRewardChart[i]["isDevelop"]["S"].ToString());
                if(isDevelopingPush && !this.isDevelop) {
                    continue;
                }
                DateTime startDateTime = DateTime.Parse(_pushRewardChart[i]["startDateTime"]["S"].ToString());
                DateTime endDateTime = DateTime.Parse(_pushRewardChart[i]["endDateTime"]["S"].ToString()).AddMinutes(1);
                
                bool isRewarded = false;
                if(this.rewardedPushDictionary.ContainsKey(pushEventId)
                    && TimeManager.Instance.CheckDateTimeInEventTime(
                        this.rewardedPushDictionary[pushEventId],
                        startDateTime, endDateTime)) {
                    isRewarded = true;
                }
                Dictionary<string, int> rewardInfoDictionary = new Dictionary<string, int>();
                PUSH_EVENT_TYPE type = (PUSH_EVENT_TYPE)Enum.Parse(typeof(PUSH_EVENT_TYPE), pushEventTypeStr);
                switch(type) {
                    case PUSH_EVENT_TYPE.SOUL_REWARD:
                        //SoulAmounts 만 RewardInfoDictionary에 추가해주시면 됩니다.
                        PushSoulReward pushSoulReward = new PushSoulReward();
                        rewardInfoDictionary.Add("SoulAmounts", int.Parse(_pushRewardChart[i]["rewardAmount"]["S"].ToString()));
                        pushSoulReward.Initialize(
                            int.Parse(_pushRewardChart[i]["num"]["S"].ToString()),
                            isRewarded, startDateTime, endDateTime, rewardInfoDictionary);
                        this.pushRewardDictionary.Add(pushEventId, pushSoulReward);
                    break;
                    default:
                        Debug.LogError("PUSH_EVENT_TYPE is not corrected! FunctionName : SavePushRewardInfo");
                    break;
                }
            }

            if(checkRewardPushesCoroutine == null) {
                checkRewardPushesCoroutine = StartCoroutine(CheckRewardPushes());
            }
        }

        private IEnumerator CheckRewardPushes() {
            yield return new WaitUntil(() => this.isLoginSuccess && this.isLoadedRewardedPushInfo && MoneyManager.instance.Loaded);

            while(true) {
                DateTime currentDateTime = DateTime.Parse(Backend.Utils.GetServerTime().GetReturnValuetoJSON()["utcTime"].ToString());
                var enumerator = this.pushRewardDictionary.GetEnumerator();
                while(enumerator.MoveNext()) {
                    if(enumerator.Current.Value.CheckAcceptable(currentDateTime)) {
                        Backend.GameInfo.Update("PushReward", this.inDateForPushReward, enumerator.Current.Value.AcceptReward(currentDateTime));
                        enumerator.Current.Value.PrintInfo();
                    }
                }

                yield return this.waitForThirtySeconds;
            }
        }
    }
}
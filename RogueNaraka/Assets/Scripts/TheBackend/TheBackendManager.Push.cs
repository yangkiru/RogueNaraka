using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackEnd;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using LitJson;
using RogueNaraka.SingletonPattern;

namespace RogueNaraka.TheBackendScripts {
    public partial class TheBackendManager : MonoSingleton<TheBackendManager> {
        private const int MAX_NUM_PUSH_REWARD_TYPE = 15;

        private Dictionary<string, bool> rewardedPushDictionary = new Dictionary<string, bool>();
        private Dictionary<string, PushEvent> pushRewardDictionary = new Dictionary<string, PushEvent>();

        private enum PROCESS_GET_PUSHFILENAME {NOT_GET, GETTING, GOT}
        private PROCESS_GET_PUSHFILENAME getFilenameProcess = PROCESS_GET_PUSHFILENAME.NOT_GET;

        private string inDateForPushReward;
        private string pushRewardChartFileName;
        private bool isGetPushRewardChartFileName;

        private void GetCahrtFimeName() {
            this.getFilenameProcess = PROCESS_GET_PUSHFILENAME.GETTING;
            Backend.Chart.GetChartList((callback) => {
                if(callback.IsSuccess()) {
                    var chartJson = callback.GetReturnValuetoJSON()["rows"];
                    for(int i = 0; i < chartJson.Count; ++i) {
                        if(chartJson[i]["chartName"]["S"].ToString() == "PushRewardChart") {
                            this.pushRewardChartFileName = chartJson[i]["selectedChartFileId"]["N"].ToString();
                            break;
                        }
                    }
                    this.getFilenameProcess = PROCESS_GET_PUSHFILENAME.GOT;
                } else {
                    this.getFilenameProcess = PROCESS_GET_PUSHFILENAME.NOT_GET;
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
                            this.rewardedPushDictionary.Add(pushId, bool.Parse(pushJson[pushId]["BOOL"].ToString()));
                        }
                    }
                }
            });
        }

        private IEnumerator GetChartAsync() {
            yield return new WaitUntil(() => this.isLoginSuccess);

            while(true) {
                switch(this.getFilenameProcess) {
                    case PROCESS_GET_PUSHFILENAME.NOT_GET:
                        GetCahrtFimeName();
                        yield return new WaitForSecondsRealtime(1.0f);
                    break;
                    case PROCESS_GET_PUSHFILENAME.GETTING:
                        yield return new WaitForSecondsRealtime(1.0f);
                    break;
                    case PROCESS_GET_PUSHFILENAME.GOT:
                        Backend.Chart.GetChartContents(this.pushRewardChartFileName, (chartContentscallback) => {
                        if(chartContentscallback.IsSuccess()) {
                                Debug.LogWarning(chartContentscallback.GetReturnValue());
                            } else {
                                this.getFilenameProcess = PROCESS_GET_PUSHFILENAME.NOT_GET;
                            }
                        });
                        if(this.getFilenameProcess == PROCESS_GET_PUSHFILENAME.GOT) {
                            yield return new WaitForSecondsRealtime(30.0f);
                        } else {
                            yield return new WaitForSecondsRealtime(10.0f);
                        }
                    break;
                }
            }
        }
    }
}
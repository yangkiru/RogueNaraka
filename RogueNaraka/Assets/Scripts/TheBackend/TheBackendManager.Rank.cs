using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using UnityEngine;
using RogueNaraka.SingletonPattern;

namespace RogueNaraka.TheBackendScripts {
    public partial class TheBackendManager : MonoSingleton<TheBackendManager> {
        private string inDateForClearedStage;
        private bool isLoadedClearedStage;
        private bool isLoadedRankData;
        private int clearedStageRank;

        public void UpdateClearedStage(int _clearedStage) {
            if(this.clearedStageRank >= _clearedStage) {
                return;
            }
            Param updateParam = new Param();
            updateParam.Add("ClearedStage", _clearedStage);
            Backend.GameInfo.Update("stage", inDateForClearedStage, updateParam);
            this.clearedStageRank = _clearedStage;
        }

        private void StartForRank() {
            StartCoroutine(LoadInDateForClearedStage());
            StartCoroutine(LoadRankData());
        }

        private IEnumerator LoadInDateForClearedStage() {
            yield return new WaitUntil(() => this.isLoginSuccess);

            Backend.GameInfo.GetPrivateContents("stage", (callback) => {
                var rankingList = callback.GetReturnValuetoJSON()["rows"];
                if(rankingList.Count == 0) {
                    Param newParam = new Param();
                    this.clearedStageRank = 0;
                    newParam.Add("ClearedStage", this.clearedStageRank);
                    Backend.GameInfo.Insert("stage", newParam, (insertCallback) => {
                        Backend.GameInfo.GetPrivateContents("stage", (reCallback) => {
                            this.inDateForClearedStage = reCallback.GetReturnValuetoJSON()["rows"][0]["inDate"]["S"].ToString();
                        });
                    });
                } else {
                    this.inDateForClearedStage = rankingList[0]["inDate"]["S"].ToString();
                    this.clearedStageRank = -5;
                    this.clearedStageRank = int.Parse(rankingList[0]["ClearedStage"]["N"].ToString());
                    Debug.LogWarningFormat("Cleared Stage : {0}", this.clearedStageRank);
                }
                this.isLoadedClearedStage = true;
            });
        }

        //RankForTier 의 uuid : "b6075f00-6a38-11e9-9a4e-79c85a29987c"
        private IEnumerator LoadRankData() {
            yield return new WaitUntil(() => this.isLoginSuccess);
            
            Backend.Rank.GetMyRank("b6075f00-6a38-11e9-9a4e-79c85a29987c", 0, (callback) => {
                if(!callback.IsSuccess()) {
                    if(callback.GetStatusCode() != "404") {
                        Debug.LogErrorFormat("Failed to Load RankData : {0} - {1}", callback.GetStatusCode(), callback.GetErrorCode());
                    }
                    Debug.Log("No Player Rank Data");
                    return;
                }

                int totalRankCount = int.Parse(callback.GetReturnValuetoJSON()["totalCount"].ToString());
            });
        }
    }
}
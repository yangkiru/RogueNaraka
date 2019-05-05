using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;
using RogueNaraka.SingletonPattern;

namespace RogueNaraka.TheBackendScripts {
    public partial class TheBackendManager : MonoSingleton<TheBackendManager> {
        private WWWForm formDataForGetRank;
        private string RequestURL = "https://roguenaraka.com/userRanking.php";
        private int clearedStageForRank;

        private bool isLoadedRankData;
        public bool IsLoadedRankData { get { return this.isLoadedRankData; } }
        private bool isRefreshing;
        public bool IsRefreshing { get { return this.isRefreshing; } }

        private float topPercentToClearStageForRank = 100.0f;
        public float TopPercentToClearStageForRank { get { return this.topPercentToClearStageForRank; } }

        public void UpdateRankData(int _clearedStage) {
            this.isRefreshing = true;
            if(this.clearedStageForRank >= _clearedStage) {
                StartCoroutine(RefreshRankDataCoroutine());
            } else {
                StartCoroutine(UploadRankDataCoroutine(_clearedStage));
            }
        }

        private void StartForRank() {
            StartCoroutine(LoadRankDataCoroutine());
        }

        private void SavedFormDataForGetRank() {
            this.formDataForGetRank = new WWWForm();
            this.formDataForGetRank.AddField("userid", this.userInDate);
            this.formDataForGetRank.AddField("command", "get");
        }

        private IEnumerator LoadRankDataCoroutine() {
            yield return new WaitUntil(() => this.isLoginSuccess && this.isSavedUserInDate);

            SavedFormDataForGetRank();
            UnityWebRequest www = UnityWebRequest.Post(
                this.RequestURL, this.formDataForGetRank);
            yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError) {
                Debug.LogError(www.error);
            } else {
                JsonData respondJson = JsonMapper.ToObject(www.downloadHandler.text);
                if(respondJson["result"].ToString() == "1") {
                    Debug.LogError("Error : Failed to Load RankData");
                } else {
                    if(respondJson["score"] != null) {
                        this.clearedStageForRank = int.Parse(respondJson["score"].ToString());
                        int total = int.Parse(respondJson["total"].ToString());
                        int ranking = total - int.Parse(respondJson["value"].ToString());
                        this.topPercentToClearStageForRank = (float)ranking / total * 100.0f;    
                    }

                    this.isLoadedRankData = true;
                }
            }
        }

        private IEnumerator UploadRankDataCoroutine(int _clearedStage) {
            yield return new WaitUntil(() => this.isLoginSuccess && this.isSavedUserInDate);

            WWWForm formData = new WWWForm();
            formData.AddField("userid", this.userInDate);
            formData.AddField("command", "set");
            formData.AddField("score", _clearedStage);
            UnityWebRequest www = UnityWebRequest.Post(
                this.RequestURL, formData);
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError) {
                Debug.LogError(www.error);
            } else {
                JsonData respondJson = JsonMapper.ToObject(www.downloadHandler.text);
                if(respondJson["result"].ToString() == "1") {
                    Debug.LogError("Error : Failed to Load RankData");
                } else {
                    this.clearedStageForRank = _clearedStage;
                    int total = int.Parse(respondJson["total"].ToString());
                    int ranking = total - int.Parse(respondJson["value"].ToString());
                    this.topPercentToClearStageForRank = (float)ranking / total * 100.0f;
                }
            }

            this.isRefreshing = false;
        }

        private IEnumerator RefreshRankDataCoroutine() {
            UnityWebRequest www = UnityWebRequest.Post(
                this.RequestURL, this.formDataForGetRank);
            yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError) {
                Debug.LogError(www.error);
            } else {
                JsonData respondJson = JsonMapper.ToObject(www.downloadHandler.text);
                if(respondJson["result"].ToString() == "1") {
                    Debug.LogError("Error : Failed to Load RankData");
                } else if(respondJson["score"] != null) {
                    this.clearedStageForRank = int.Parse(respondJson["score"].ToString());
                    int total = int.Parse(respondJson["total"].ToString());
                    int ranking = total - int.Parse(respondJson["value"].ToString());
                    this.topPercentToClearStageForRank = (float)ranking / total * 100.0f;
                }
            }

            this.isRefreshing = false;
        }
    }
}
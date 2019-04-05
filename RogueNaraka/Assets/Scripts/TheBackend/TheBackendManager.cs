using System.Collections;
using System.Collections.Generic;
using BackEnd;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using RogueNaraka.SingletonPattern;

namespace RogueNaraka.TheBackendScripts {
    public class TheBackendManager : MonoSingleton<TheBackendManager> {
        public override void OnDestroy() {
            base.OnDestroy();
        }

        void Awake() {
            #if UNITY_EDITOR
                this.gameObject.SetActive(false);
                return;
            #endif
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
                .Builder()
                .RequestServerAuthCode(false)
                .RequestIdToken()
                .RequestEmail()
                .Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
        }

        void Start() {
            if(!Backend.IsInitialized) {
                Backend.Initialize(BRO => {
                    if(BRO.IsSuccess()) {
                        BackendInit();
                    } else {
                        Debug.LogError("Backend Initialize Failed");
                    }
                });
            } else {
                BackendInit();
            }

            Invoke("AuthorizeFederationSync", 0.5f);
        }
        
        //bool tmpcheck = false;
        //bool tmploginsuccess = false;
        void Update() {
            /*
            if(!tmpcheck && tmploginsuccess) {
                Backend.Event.EventList((list) => {
                    var eventlist = Backend.Event.EventList().GetReturnValuetoJSON();
                    Debug.LogError("Event!");
                    Debug.Log(eventlist["rows"][0].ToJson());
                });
                tmpcheck = true;
            }*/
        }

        private void BackendInit() {
            Debug.Log(Backend.Utils.GetServerTime());
        }

        private void AuthorizeFederationSync() {
            //안드로이드
            GPGSLogin();
            //

        }

        private void GPGSLogin() {
            if(Social.localUser.authenticated == true) {
                BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(GetTokens(), FederationType.Google, "gpgs");
                WorkAfterGPGSLogin();
            } else {
                Social.localUser.Authenticate((bool success) => {
                    if(success) {
                        BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(GetTokens(), FederationType.Google, "gpgs");
                        WorkAfterGPGSLogin();
                    } else {
                        Debug.LogError("GPGS Login Failed");
                    }
                });
            }
        }

        private void WorkAfterGPGSLogin() {
            //접속 체크
            //Debug.LogError(Backend.BMember.AuthorizeFederation(GetTokens(), FederationType.Google));
            //푸시 설정
            Backend.Android.PutDeviceToken();
            //tmploginsuccess = true;
        }

        private string GetTokens() {
            if(PlayGamesPlatform.Instance.localUser.authenticated) {
                string _IDtoken = PlayGamesPlatform.Instance.GetIdToken();
                return _IDtoken;
            } else {
                Debug.Log("접속되어있지 않습니다. PlayGamesPlatform.Instance.localUser.authenticated :  fail");
                return null;
            }
        }
    }
}
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
        private bool isLoginSuccess;
        public bool IsLoginSuccess { get { return this.isLoginSuccess; } }

        private string userInDate;
        public string UserInDate { get { return this.userInDate; } }

        private bool isSavedUserInDate;

        void AwakeForLogin() {
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

        void StartForLogin() {
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
            Debug.Log("Logined!");
            Backend.Android.PutDeviceToken();
            this.isLoginSuccess = true;
            this.userInDate = Backend.BMember.GetUserInfo().GetReturnValuetoJSON()["row"]["inDate"].ToString();
            this.isSavedUserInDate = true;
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
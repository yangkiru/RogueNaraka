using System.Collections;
using System.Collections.Generic;
using BackEnd;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using RogueNaraka.SingletonPattern;
using RogueNaraka.NotificationScripts;

namespace RogueNaraka.TheBackendScripts {
    public partial class TheBackendManager : MonoSingleton<TheBackendManager> {
        public TextMeshProUGUI logoutBtnText;

        private bool isLoginSuccess;
        public bool IsLoginSuccess { get { return this.isLoginSuccess; } }

        private string userInDate;
        public string UserInDate { get { return this.userInDate; } }

        private bool isSavedUserInDate;
        public bool IsSavedUserInDate { get { return this.isSavedUserInDate; } }

        private bool isLogout;
        public bool IsLogout { get { return this.isLogout; } }

        void AwakeForLogin() {
            ActivatePlayGamesPlatform();
        }

        void StartForLogin() {
            if(!Backend.IsInitialized) {
                Backend.Initialize(BRO => {
                    if(BRO.IsSuccess()) {
                        BackendInit();
                        AuthorizeFederationSync();
                    } else {
                        Debug.LogError("Backend Initialize Failed");
                    }
                });
            } else {
                BackendInit();
                AuthorizeFederationSync();
            }
        }

        private void ActivatePlayGamesPlatform() {
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
                        //로그인 실패
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
            this.isLoginSuccess = true;
            this.userInDate = Backend.BMember.GetUserInfo().GetReturnValuetoJSON()["row"]["inDate"].ToString();
            this.isSavedUserInDate = true;
            Backend.Android.PutDeviceToken();
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

        ///<summary>Click Logout Button</summary>
        public void ClickOnLogoutButton() {
            if(!this.isLogout) {
                //Logout
                Backend.BMember.Logout();
                ((PlayGamesPlatform)Social.Active).SignOut();
                this.logoutBtnText.text = "Login";
                this.isLogout = true;
                PlayerPrefs.SetInt("IsLogout", 1);
                switch(GameManager.language) {
                    case Language.English:
                        NotificationWindowManager.Instance.EnqueueNotificationData("You've logged out.");
                    break;
                    case Language.Korean:
                        NotificationWindowManager.Instance.EnqueueNotificationData("로그아웃 했습니다.");
                    break;
                }
                
            } else {
                //Login
                ActivatePlayGamesPlatform();
                StartForLogin();
                this.logoutBtnText.text = "Logout";
                this.isLogout = false;
                PlayerPrefs.SetInt("IsLogout", 0);
                switch(GameManager.language) {
                    case Language.English:
                        NotificationWindowManager.Instance.EnqueueNotificationData("You're logged in.");
                    break;
                    case Language.Korean:
                        NotificationWindowManager.Instance.EnqueueNotificationData("로그인 했습니다.");
                    break;
                }
            }
        }
    }
}
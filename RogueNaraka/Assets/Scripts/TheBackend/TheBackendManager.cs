using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;
using RogueNaraka.SingletonPattern;

namespace RogueNaraka.TheBackendScripts {
    public partial class TheBackendManager : MonoSingleton<TheBackendManager> {
        public override void OnDestroy() {
            base.OnDestroy();
        }

        private WaitForSecondsRealtime waitForOneSeconds = new WaitForSecondsRealtime(1.0f);
        private WaitForSecondsRealtime waitForTenSeconds = new WaitForSecondsRealtime(10.0f);
        private WaitForSecondsRealtime waitForThirtySeconds = new WaitForSecondsRealtime(30.0f);

        void Awake() {
            #if UNITY_EDITOR
                this.gameObject.SetActive(false);
                return;
            #endif
        }

        void Start() {
            if(PlayerPrefs.GetInt("IsLogout") == 0) {
                StartForLogin();
                StartForPush();
                StartForRank();
                this.isLogout = false;
                this.logoutBtnText.text = "Logout";
            } else {
                StartForRankWithoutGPSLogin();
                this.isLogout = true;
                this.logoutBtnText.text = "Login";
            }
        }

        public void StopBackend() {
            StopForPush();
        }

        private void BackendInit() {
            Debug.Log(Backend.Utils.GetServerTime());
        }

        
    }
}
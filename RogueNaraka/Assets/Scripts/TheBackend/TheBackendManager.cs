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
            AwakeForLogin();
        }

        void Start() {
            StartForLogin();
            StartForPush();
        }

        private void BackendInit() {
            Debug.Log(Backend.Utils.GetServerTime());
        }
    }
}
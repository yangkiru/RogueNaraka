using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using RogueNaraka.SingletonPattern;
using RogueNaraka.TimeScripts;
using RogueNaraka.TheBackendScripts;
using RogueNaraka.PopUpScripts;

namespace RogueNaraka.TitleScripts {
    public class TitleManager : SingletonPattern.MonoSingleton<TitleManager> {
        public Fade Fade;
        public GameObject Grid;
        public TextMeshProUGUI ProcessText;

        private bool isAbleToGoMain;

        private void Start() {
            StartCoroutine(CheckToCanGoMain());
        }

        public void OnClick() {
            if(!isAbleToGoMain) {
                return;
            }

            AdMobManager.instance.RequestBanner();
            AudioManager.instance.PlaySFX("gameStart");
            this.Fade.FadeOut();
            this.Grid.SetActive(true);
        }

        private IEnumerator CheckToCanGoMain() {
            WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
            this.ProcessText.text = "Logining...";
            float time = 0.0f;

            yield return waitForFixedUpdate;
            #if !UNITY_EDITOR
            while(true) {
                time += TimeManager.Instance.FixedDeltaTime;
                if(TheBackendManager.Instance.IsSavedUserInDate) {
                    break;
                } else if(time > 20.0f) {
                    string context = "";
                    switch(GameManager.language) {
                        case Language.English:
                            context = "You are in Offline mode because of bad network connection.";
                        break;
                        case Language.Korean:
                            context = "연결이 원할하지 않아 오프라인 모드로 진행합니다.";
                        break;
                    }
                    PopUpManager.Instance.ActivateOneButtonPopUp(context,
                        (OneButtonPopUpController _popUp) => { 
                            _popUp.DeactivatePopUp(); 
                            PopUpManager.Instance.DeactivateBackPanel();
                            GameManager.instance.SetPause(false);
                        });
                    TheBackendManager.Instance.gameObject.SetActive(false);
                    break;
                }
                yield return waitForFixedUpdate;
            }
            #endif

            this.ProcessText.text = "Touch To Start";
            this.isAbleToGoMain = true;
        } 
    }
}
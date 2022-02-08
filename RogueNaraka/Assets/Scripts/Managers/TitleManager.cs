using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using RogueNaraka.SingletonPattern;
using RogueNaraka.TimeScripts;
//using RogueNaraka.TheBackendScripts;
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

            this.ProcessText.text = "Touch To Start";
            this.isAbleToGoMain = true;
        } 
    }
}
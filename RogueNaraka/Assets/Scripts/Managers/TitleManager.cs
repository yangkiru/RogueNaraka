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

        public void OnClick() {
            //AdMobManager.instance.RequestBanner();
            AudioManager.instance.PlaySFX("gameStart");
            this.Fade.FadeOut();
        }

        public void OnFadeOutEnd(){
            Fade.FadeIn();
            LobbyManager.Instance.Content.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
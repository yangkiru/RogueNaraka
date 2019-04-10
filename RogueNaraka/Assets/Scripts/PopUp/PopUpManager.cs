using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RogueNaraka.SingletonPattern;

namespace RogueNaraka.PopUpScripts {
    public class PopUpManager : MonoSingleton<PopUpManager> {
        public Image BackPanel;
        public OneButtonPopUpController OneButtonPopUp;
        public OneButtonVerticalScrollPopUpController OneButtonVerticalScrollPopUp;
        public PrivacyPolicyPopUpController PrivacyPolicyPopUp;

        void Start() {
            if(PlayerPrefs.GetInt("PrivacyPolicy") != 1) {
                //개인정보 처리방침 팝업
                GameManager.instance.SetPause(true);
                this.BackPanel.gameObject.SetActive(true);
                this.PrivacyPolicyPopUp.ActivatePopUp();
                //
            }
        }

        /// <summary>Action 에 들어가는 인자는 팝업 자기 자신입니다. 해당 인자에 버튼을 클릭시 실행되는 함수를 입력하시면 됩니다.</summary>
        public void ActivateOneButtonPopUp(string _context, Action<OneButtonPopUpController> _action) {
            GameManager.instance.SetPause(true);
            this.BackPanel.gameObject.SetActive(true);
            this.OneButtonPopUp.SetPopUpData(_context, _action);
            this.OneButtonPopUp.ActivatePopUp();
        }

        /// <summary>Action 에 들어가는 인자는 팝업 자기 자신입니다. 해당 인자에 버튼을 클릭시 실행되는 함수를 입력하시면 됩니다.</summary>
        public void ActivateOneButtonPopUp(string _context, Action<OneButtonVerticalScrollPopUpController> _action) {
            GameManager.instance.SetPause(true);
            this.BackPanel.gameObject.SetActive(true);
            this.OneButtonVerticalScrollPopUp.SetPopUpData(_context, _action);
            this.OneButtonVerticalScrollPopUp.ActivatePopUp();
        }

        public void DeactivateBackPanel() {
            this.BackPanel.gameObject.SetActive(false);
        }
    }
}


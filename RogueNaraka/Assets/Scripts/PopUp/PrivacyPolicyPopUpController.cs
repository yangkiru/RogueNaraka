using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RogueNaraka.PopUpScripts {
    public class PrivacyPolicyPopUpController : PopUpController {
        public TextMeshProUGUI Context;
        public TextMeshProUGUI ToggleText;
        public TextMeshProUGUI ButtonText;
        public Toggle toggle;
        public Button button;
        public Image buttonMask;

        void Start() {
            switch(GameManager.language) {
                case Language.Korean:
                    Context.text = Resources.Load<TextAsset>("PrivacyPolicy/PrivacyPolicy_kor").text;
                    ToggleText.text = "개인정보취급방침을 읽었으며, 이에 동의합니다.";
                    ButtonText.text = "동의함";
                break;
                case Language.English:
                    Context.text = Resources.Load<TextAsset>("PrivacyPolicy/PrivacyPolicy_eng").text;
                    ToggleText.text = "I have read and agree to the Privacy Policy.";
                    ButtonText.text = "Agree";
                break;
            }
        }

        public void OnClickToggle() {
            if(this.toggle.isOn) {
                this.buttonMask.color = new Color(this.buttonMask.color.r, this.buttonMask.color.g, this.buttonMask.color.b, 0.0f); 
            } else {
                this.buttonMask.color = new Color(this.buttonMask.color.r, this.buttonMask.color.g, this.buttonMask.color.b, 0.5f);
            }
        }

        public void OnClickButton() {
            if(this.toggle.isOn) {
                DeactivatePopUp();
            }
        }

        public override void DeactivatePopUp() {
            this.gameObject.SetActive(false);
            PlayerPrefs.SetInt("PrivacyPolicy", 1);
            PopUpManager.Instance.DeactivateBackPanel();
            GameManager.instance.SetPause(false);
        }
    }
}
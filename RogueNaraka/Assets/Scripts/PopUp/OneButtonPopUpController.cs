using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RogueNaraka.PopUpScripts {
    public class OneButtonPopUpController : MonoBehaviour {
        public TextMeshProUGUI Context;
        public Button Button;

        public void SetPopUpData(string _context) {
            this.Context.text = _context;;
        }

        public void OnClickButton() {
            this.gameObject.SetActive(false);
            PopUpManager.Instance.DeActivateBackPanel();
        }
    }
}
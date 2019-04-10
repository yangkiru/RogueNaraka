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

        public void ActivateOneButtonPopUp(string _context) {
            this.BackPanel.gameObject.SetActive(true);
            this.OneButtonPopUp.SetPopUpData(_context);
            this.OneButtonPopUp.gameObject.SetActive(true);
        }

        public void DeActivateBackPanel() {
            this.BackPanel.gameObject.SetActive(false);
        }
    }
}


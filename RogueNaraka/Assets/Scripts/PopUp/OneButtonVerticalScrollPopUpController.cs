using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RogueNaraka.PopUpScripts {
    public class OneButtonVerticalScrollPopUpController : PopUpController {
        public TextMeshProUGUI Context;
        public Button Button;

        public Action<OneButtonVerticalScrollPopUpController> buttonAction;

        /// <summary>Action 에 들어가는 인자는 팝업 자기 자신입니다.</summary>
        public void SetPopUpData(string _context, Action<OneButtonVerticalScrollPopUpController> _action) {
            this.Context.text = _context;
            this.buttonAction = _action;
        }

        public void OnClickButton() {
            this.buttonAction(this);
        }
    }
}
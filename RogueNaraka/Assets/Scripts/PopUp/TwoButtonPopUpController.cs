using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RogueNaraka.PopUpScripts {
    public class TwoButtonPopUpController : PopUpController {
        public TextMeshProUGUI Context;
        public Button Button1;
        public TextMeshProUGUI Button1Text1;
        public Button Button2;
        public TextMeshProUGUI Button1Text2;

        public Action<TwoButtonPopUpController> buttonAction1;
        public Action<TwoButtonPopUpController> buttonAction2;

        /// <summary>Action 에 들어가는 인자는 팝업 자기 자신입니다.</summary>
        public void SetPopUpData(string _context, string _buttonText1, Action<TwoButtonPopUpController> _action1
            , string _buttonText2, Action<TwoButtonPopUpController> _action2) {
            this.Context.text = _context;

            this.Button1Text1.text = _buttonText1;
            this.buttonAction1 = _action1;

            this.Button1Text2.text = _buttonText2;
            this.buttonAction2 = _action2;
        }

        public void OnClickButton1() {
            this.buttonAction1(this);
        }

        public void OnClickButton2() {
            this.buttonAction2(this);
        }
    }
}
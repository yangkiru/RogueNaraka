using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RogueNaraka.SingletonPattern;

namespace RogueNaraka.PopUpScripts {
    public class PopUpManager : MonoSingleton<PopUpManager> {
        public Image BackPanel;
        public OneButtonPopUpController OneButtonPopUp;
    }
}


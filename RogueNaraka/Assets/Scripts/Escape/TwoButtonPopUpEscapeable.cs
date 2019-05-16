using UnityEngine;
using System.Collections;
using RogueNaraka.PopUpScripts;

namespace RogueNaraka.Escapeable
{
    public class TwoButtonPopUpEscapeable : Escapeable
    {
        public EscapeEvent onTwoBtn1;
        public EscapeEvent onTwoBtn2;
        public EscapeEvent onEscapeOnPopUp;

        [TextArea]
        public string[] twoPopUpContext;
        [TextArea]
        public string[] twoPopUpTxt1;
        [TextArea]
        public string[] twoPopUpTxt2;

        public void SetTwoButtonPopUpAction(TwoButtonPopUpController popup)
        {
            PopUpManager.Instance.ActivateTwoButtonPopUp(GetLangText(twoPopUpContext), GetLangText(twoPopUpTxt1), (TwoButtonPopUpController _popup) =>
            {
                if(onTwoBtn1 != null)
                    onTwoBtn1.Invoke();
            },
            GetLangText(twoPopUpTxt2),
             (TwoButtonPopUpController _popup) =>
             {
                 if (onTwoBtn2 != null)
                     onTwoBtn2.Invoke();
             }
            );
            Escapeable escapeable = popup.GetComponent<Escapeable>();
            escapeable.onEscape = onEscapeOnPopUp;
            escapeable.onEscape.AddListener(() => ClearOnEscape(escapeable));
        }

        public void OnTwoBtn1()
        {
            if (onTwoBtn1 != null)
                onTwoBtn1.Invoke();
        }

        public void OnTwoBtn2()
        {
            if (onTwoBtn2 != null)
                onTwoBtn2.Invoke();
        }

        string GetLangText(string[] texts)
        {
            if (texts.Length == 0)
                return string.Empty;
            int lang = (int)GameManager.language;
            if (lang > texts.Length)
                lang = 0;
            return texts[lang];
            
        }
    }
}
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RogueNaraka.Escapeable
{
    public class ButtonEscapeable : Escapeable
    {
        public Button.ButtonClickedEvent onClick;

        public void SetOnClick(Button btn)
        {
            btn.onClick = onClick;
        }
    }
}
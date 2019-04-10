using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.PopUpScripts {
    public abstract class PopUpController : MonoBehaviour {
        public virtual void ActivatePopUp() {
            this.gameObject.SetActive(true);
        }

        public virtual void DeactivatePopUp() {
            this.gameObject.SetActive(false);
        }
    }
}
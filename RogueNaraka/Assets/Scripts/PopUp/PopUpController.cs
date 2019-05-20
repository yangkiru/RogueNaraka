using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RogueNaraka.PopUpScripts {
    public abstract class PopUpController : MonoBehaviour {
        private RectTransform myRectTransform;

        void Awake() {
            this.myRectTransform = this.transform as RectTransform;
        }

        public virtual void ActivatePopUp() {
            this.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.myRectTransform);
        }

        public virtual void DeactivatePopUp() {
            this.gameObject.SetActive(false);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.ShadowScripts {
    public class ShadowController : MonoBehaviour {
        public SpriteRenderer UnitRenderer;
        public SpriteRenderer ShadowRenderer;

        void LateUpdate() {
            if(this.ShadowRenderer.sprite != this.UnitRenderer.sprite) {
                this.ShadowRenderer.sprite = this.UnitRenderer.sprite;
            }
        }

        public void Initialize(float _angleZ) {
            this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, _angleZ);
        }
    }
}
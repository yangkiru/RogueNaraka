using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.ShadowScripts {
    public enum SHADOW_TYPE {
        ONE_WAY,
        TWO_WAY,
        FOUR_WAY,
        EIGHT_WAY,
        END
    }
    public class ShadowController : MonoBehaviour {
        public SpriteRenderer UnitRenderer;
        public Animator UnitAnimator;
        public SpriteRenderer ShadowRenderer;

        private WayShadowChecker wayChecker;

        void LateUpdate() {
            if(this.ShadowRenderer.sprite.name != this.UnitRenderer.sprite.name) {
                
                if (this.wayChecker != null)
                {
                    this.ShadowRenderer.sprite = this.UnitRenderer.sprite;
                    this.ShadowRenderer.enabled = true;
                    this.wayChecker.CheckWay(UnitAnimator.GetFloat("x"), UnitAnimator.GetFloat("y"));
                }
                else
                    this.ShadowRenderer.enabled = false;
            }
        }

        public void Initialize(float _angleZ, bool _flipX, Vector2[] _posArr) {
            this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, _angleZ);
            this.ShadowRenderer.flipX = _flipX;
            switch(_posArr.Length) {
                case 0:
                    this.wayChecker = null;
                break;
                case 1:
                    this.wayChecker = new OneWayShadowChecker(this, _posArr);
                break;
                case 2:
                    this.wayChecker = new TwoWayShadowChecker(this, _posArr);
                break;
                case 4:
                    this.wayChecker = new FourWayShadowChecker(this, _posArr);
                break;
                case 8:
                    this.wayChecker = new EightWayShadowChecker(this, _posArr);
                break;
            }
        }
    }
}
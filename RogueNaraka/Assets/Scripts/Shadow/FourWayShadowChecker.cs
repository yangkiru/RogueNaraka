using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.ShadowScripts {
    enum FOUR_WAY_TYPE {
        UP_LEFT,
        DOWN_LEFT,
        DOWN_RIGHT,
        UP_RIGHT,
        END
    }
    public partial class FourWayShadowChecker : WayShadowChecker {
        private FOUR_WAY_TYPE way;

        public FourWayShadowChecker (ShadowController _shadow, Vector2[] _posArr) 
         : base (_shadow, _posArr) {
            this.type = SHADOW_TYPE.FOUR_WAY;
            this.way = FOUR_WAY_TYPE.END;
        }

        public override void CheckWay(float _wayX, float _wayY) {
            Vector2 animatorWay = new Vector2(_wayX, _wayY).normalized;
            Vector2 pivotWay = new Vector2(0.0f, 1.0f).normalized;
            float degree = Vector2.Angle(animatorWay, pivotWay);
            if(animatorWay.x < 0.0f) {
                if(degree < 45.0f) {
                    this.way = FOUR_WAY_TYPE.UP_LEFT;
                } else {
                    this.way = FOUR_WAY_TYPE.DOWN_LEFT;
                }
            } else {
                if(degree < 45.0f) {
                    this.way = FOUR_WAY_TYPE.UP_RIGHT;
                } else {
                    this.way = FOUR_WAY_TYPE.DOWN_RIGHT;
                }
            }
            this.shadow.transform.localPosition = this.posArr[(int)this.way];
        }
    }
}
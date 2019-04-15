using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.ShadowScripts {
    enum TWO_WAY_TYPE {
        LEFT,
        RIGHT,
        END
    }
    public partial class TwoWayShadowChecker : WayShadowChecker {
        private TWO_WAY_TYPE way;

        public TwoWayShadowChecker (ShadowController _shadow, Vector2[] _posArr) 
         : base (_shadow, _posArr) {
            this.type = SHADOW_TYPE.TWO_WAY;
            this.way = TWO_WAY_TYPE.END;
        }

        public override void CheckWay(float _wayX, float _wayY) {
            Vector2 animatorWay = new Vector2(_wayX, _wayY).normalized;
            Vector2 pivotWay = new Vector2(0.0f, 1.0f).normalized;
            float degree = Vector2.Angle(animatorWay, pivotWay);
            if(animatorWay.x < 0.0f) {
                this.way = TWO_WAY_TYPE.LEFT;
            } else {
                this.way = TWO_WAY_TYPE.RIGHT;
            }
            this.shadow.transform.localPosition = this.posArr[(int)this.way];
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.ShadowScripts {
    public partial class OneWayShadowChecker : WayShadowChecker {
        public OneWayShadowChecker (ShadowController _shadow, Vector2[] _posArr) 
         : base (_shadow, _posArr) {
            this.type = SHADOW_TYPE.ONE_WAY;
            this.shadow.transform.localPosition = _posArr[0];
        }

        public override void CheckWay(float _wayX, float _wayY) {}
    }
}
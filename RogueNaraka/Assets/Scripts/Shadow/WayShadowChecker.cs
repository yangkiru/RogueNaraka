using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.ShadowScripts {
    public abstract class WayShadowChecker {
        protected ShadowController shadow;
        protected SHADOW_TYPE type;
        protected Vector2[] posArr;

        public WayShadowChecker (ShadowController _shadow, Vector2[] _posArr) {
            this.shadow = _shadow;
            this.posArr = _posArr.Clone() as Vector2[];
        }

        public abstract void CheckWay(float _wayX, float _wayY);
    }
}
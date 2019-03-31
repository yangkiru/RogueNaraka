using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.ExtensionMethod {
    public static class VectorExtensionMethod {
        /// <summary> Vector2인 값에서 Vector3인 _value를 뺀 값을 리턴합니다.</summary>
        public static Vector2 SubtractVector3FromVector2(this Vector2 _this, Vector3 _value) {
            return new Vector2(_this.x - _value.x, _this.y - _value.y);
        }
    }
}
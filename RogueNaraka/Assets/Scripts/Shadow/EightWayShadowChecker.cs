using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.ShadowScripts {
    enum EIGHT_WAY_TYPE {
        UP,
        UP_LEFT,
        LEFT,
        DOWN_LEFT,
        DOWN,
        DOWN_RIGHT,
        RIGHT,
        UP_RIGHT,
        END
    }
    public partial class EightWayShadowChecker : WayShadowChecker {
        private EIGHT_WAY_TYPE way;

        public EightWayShadowChecker (ShadowController _shadow, Vector2[] _posArr) 
         : base (_shadow, _posArr) {
            this.type = SHADOW_TYPE.EIGHT_WAY;
            this.way = EIGHT_WAY_TYPE.END;
        }

        public override void CheckWay(float _wayX, float _wayY) {
            Vector2 animatorWay = new Vector2(_wayX, _wayY).normalized;
            Vector2 pivotWay = new Vector2(0.0f, 1.0f).normalized;
            float degree = Vector2.Angle(animatorWay, pivotWay);
            if(animatorWay.x < 0.0f) {
                if(degree < 22.5f) {
                    this.way = EIGHT_WAY_TYPE.UP;
                } else if(degree < 67.5f) {
                    this.way = EIGHT_WAY_TYPE.UP_LEFT;
                } else if(degree < 112.5f){
                    this.way = EIGHT_WAY_TYPE.LEFT;
                } else if(degree < 157.5f){
                    this.way = EIGHT_WAY_TYPE.DOWN_LEFT;
                } else {
                    this.way = EIGHT_WAY_TYPE.DOWN;
                }
            } else {
                if(degree < 22.5f) {
                    this.way = EIGHT_WAY_TYPE.UP;
                } else if(degree < 67.5f) {
                    this.way = EIGHT_WAY_TYPE.UP_RIGHT;
                } else if(degree < 112.5f){
                    this.way = EIGHT_WAY_TYPE.RIGHT;
                } else if(degree < 157.5f){
                    this.way = EIGHT_WAY_TYPE.DOWN_RIGHT;
                } else {
                    this.way = EIGHT_WAY_TYPE.DOWN;
                }
            }
            this.shadow.transform.localPosition = this.posArr[(int)this.way];
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.SingletonPattern;

namespace RogueNaraka.TierScripts {
    public class TierManager : MonoSingleton<TierManager> {
        private int playerLevel;
        public int PlayerLevel { get { return this.playerLevel; } }
        private float currentExp;
        public float CurrentExp { get { return this.currentExp; } }
        private float totalGainExpInGame;
        public float TotalGainExpInGame { get { return this.totalGainExpInGame; } }

        /// <summary> 해당 클래스 사용 종료 시 반드시 해당 함수를 실행해주세요. </summary>
        public override void OnDestroy() {
            base.OnDestroy();
        }

        public void GainExp(float _exp) {
            this.totalGainExpInGame += _exp;
        }
    }
}
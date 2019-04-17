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

        void Start() {
            //레벨 세팅
            this.playerLevel = PlayerPrefs.GetInt("PlayerLv");
            this.currentExp = PlayerPrefs.GetFloat("PlayerExp");
            if(this.playerLevel == 0) {
                this.playerLevel = 1;
                this.currentExp = 0.0f;
            }
            //test
            //this.playerLevel = 1;
            //this.currentExp = 0.0f;
        }

        public void GainExp(float _exp) {
            this.totalGainExpInGame += _exp;
        }

        public void SaveExp() {
            //test
            //this.totalGainExpInGame = 2000.0f;
            //
            this.currentExp += this.totalGainExpInGame;
            while(this.currentExp >= GameDatabase.instance.requiredExpTable[this.playerLevel - 1]) {
                this.currentExp -= GameDatabase.instance.requiredExpTable[this.playerLevel - 1];
                this.playerLevel++;
            }
            PlayerPrefs.SetInt("PlayerLv", this.playerLevel);
            PlayerPrefs.SetFloat("PlayerExp", this.currentExp);
        }
    }
}
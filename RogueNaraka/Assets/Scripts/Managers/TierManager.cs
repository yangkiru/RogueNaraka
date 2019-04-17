using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.SingletonPattern;

namespace RogueNaraka.TierScripts {
    public class TierManager : MonoSingleton<TierManager> {
        private int playerLevel;
        public int PlayerLevel { get { return this.playerLevel; } }
        private double currentExp;
        public double CurrentExp { get { return this.currentExp; } }
        private double totalGainExpInGame;
        public double TotalGainExpInGame { get { return this.totalGainExpInGame; } }

        /// <summary> 해당 클래스 사용 종료 시 반드시 해당 함수를 실행해주세요. </summary>
        public override void OnDestroy() {
            base.OnDestroy();
        }

        void Start() {
            //레벨 세팅
            this.playerLevel = PlayerPrefs.GetInt("PlayerLv");
            if(PlayerPrefs.GetString("PlayerExp") != "") {
                this.currentExp = double.Parse(PlayerPrefs.GetString("PlayerExp"));
            }
            if(this.playerLevel == 0) {
                this.playerLevel = 1;
                this.currentExp = 0.0d;
            }
            //test
            this.playerLevel = 1;
            this.currentExp = 0.0d;
        }

        public void GainExp(float _exp) {
            this.totalGainExpInGame += _exp;
        }

        public void SaveExp() {
            //test
            this.totalGainExpInGame = 2000.0d;
            //
            this.currentExp += this.totalGainExpInGame;
            while(this.currentExp >= GameDatabase.instance.requiredExpTable[this.playerLevel - 1]) {
                this.currentExp -= GameDatabase.instance.requiredExpTable[this.playerLevel - 1];
                this.playerLevel++;
            }
            PlayerPrefs.SetInt("PlayerLv", this.playerLevel);
            PlayerPrefs.SetString("PlayerExp", this.currentExp.ToString());
        }

        //절대 Required EXP Table 데이터 변경 이외의 용도로 사용하지 마세요!!
        private void SetRequiredExpTable() {
            GameDatabase.instance.requiredExpTable[0] = 150.0d;
            for(int i = 1; i < GameDatabase.instance.requiredExpTable.Length; ++i) {
                GameDatabase.instance.requiredExpTable[i] = System.Math.Round(GameDatabase.instance.requiredExpTable[i-1] * 1.333d);
            }
        }
    }
}
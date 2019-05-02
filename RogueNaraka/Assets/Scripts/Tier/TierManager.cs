using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.SingletonPattern;
using RogueNaraka.TheBackendScripts;
using RogueNaraka.NotificationScripts;

namespace RogueNaraka.TierScripts {
    public class TierManager : MonoSingleton<TierManager> {
        private bool isLoaded;
        public bool IsLoaded { get { return this.isLoaded; } }
        //레벨
        private int playerLevel;
        public int PlayerLevel { get { return this.playerLevel; } }
        private double currentExp;
        public double CurrentExp { get { return this.currentExp; } }
        private double totalGainExpInGame;
        public double TotalGainExpInGame { get { return this.totalGainExpInGame; } }
        //티어
        private TierData currentTier;
        public TierData CurrentTier { get { return this.currentTier; } }
        private bool isAdvanced;
        public bool IsAdvanced { get { return this.isAdvanced; } }
        private bool isCheckedToChangeTier;
        public bool IsCheckedToChangeTier { get { return this.isCheckedToChangeTier; } }

        private int currentTierIdx;

        public TierData NextTier { get { return GameDatabase.instance.TierDataes[this.currentTierIdx + 1]; } }

        /// <summary> 해당 클래스 사용 종료 시 반드시 해당 함수를 실행해주세요. </summary>
        public override void OnDestroy() {
            base.OnDestroy();
        }

        void Start() {
            LoadData();
            #if !UNITY_EDITOR
                StartCoroutine(CheckIfTierHaveChangedForGameStart());
            #endif
        }
        
        public void GainExp(float _exp) {
            this.totalGainExpInGame += _exp;
        }

        public void SaveExp() {
            this.currentExp += this.totalGainExpInGame;
            while(this.currentExp >= GameDatabase.instance.requiredExpTable[this.playerLevel - 1]) {
                this.currentExp -= GameDatabase.instance.requiredExpTable[this.playerLevel - 1];
                this.playerLevel++;
            }
            PlayerPrefs.SetInt("PlayerLv", this.playerLevel);
            PlayerPrefs.SetString("PlayerExp", this.currentExp.ToString());
        }

        public bool CheckIfTierHaveChanged() {
            if(GameDatabase.instance.TierDataes[this.currentTierIdx + 1].requiredRankingPercent < TheBackendManager.Instance.TopPercentToClearStageForRank) {
                if(GameDatabase.instance.TierDataes[this.currentTierIdx].requiredRankingPercent >= TheBackendManager.Instance.TopPercentToClearStageForRank) {
                    return false;
                }
                for(int i = this.currentTierIdx; i >= 0; --i) {
                    if(GameDatabase.instance.TierDataes[i].requiredRankingPercent >= TheBackendManager.Instance.TopPercentToClearStageForRank) {
                        this.currentTierIdx = i;
                        this.currentTier = GameDatabase.instance.TierDataes[i];
                        PlayerPrefs.SetInt("PlayerTier", this.currentTierIdx);
                        this.isAdvanced = false;
                        return true;
                    }
                }
            } else {
                for(int i = this.currentTierIdx + 1; i < GameDatabase.instance.TierDataes.Length - 1; ++i) {
                    if(GameDatabase.instance.TierDataes[i + 1].requiredRankingPercent < TheBackendManager.Instance.TopPercentToClearStageForRank) {
                        this.currentTierIdx = i;
                        this.currentTier = GameDatabase.instance.TierDataes[i];
                        PlayerPrefs.SetInt("PlayerTier", this.currentTierIdx);
                        this.isAdvanced = true;
                        return true;
                    }
                }
            }
            
            throw new System.IndexOutOfRangeException("Failed to Find Correct Tier");
        }

        public IEnumerator CheckIfTierHaveChangedForGameStart() {
            yield return new WaitUntil(() => this.isLoaded && TheBackendManager.Instance.IsLoadedRankData);
            
            if(CheckIfTierHaveChanged()) {
                string textFormat = "";
                if(this.isAdvanced) {
                    switch(GameManager.language) {
                        case Language.English:
                            textFormat = "Promoted to {0} {1}!";
                        break;
                        case Language.Korean:
                            textFormat = "{0} {1}로 승급했습니다!";
                        break;
                    }
                } else {
                    switch(GameManager.language) {
                        case Language.English:
                            textFormat = "Downgraded to {0} {1}.";
                        break;
                        case Language.Korean:
                            textFormat = "{0} {1}로 강등했습니다.";
                        break;
                    }
                }
                NotificationWindowManager.Instance.EnqueueNotificationData(
                    string.Format(textFormat, this.currentTier.type
                        , this.CurrentTier.tier_num != 0 ? this.CurrentTier.tier_num.ToString() : "" ));
            }
            this.isCheckedToChangeTier = true;
        }

        private void LoadData() {
            //레벨 세팅
            this.playerLevel = PlayerPrefs.GetInt("PlayerLv");
            if(PlayerPrefs.GetString("PlayerExp") != "") {
                this.currentExp = double.Parse(PlayerPrefs.GetString("PlayerExp"));
            }
            if(this.playerLevel == 0) {
                this.playerLevel = 1;
                this.currentExp = 0.0d;
            }
            //티어 세팅
            this.currentTierIdx = PlayerPrefs.GetInt("PlayerTier");
            this.currentTier = GameDatabase.instance.TierDataes[this.currentTierIdx];
            //
            this.isLoaded = true;
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